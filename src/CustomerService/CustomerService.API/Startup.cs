using AspNetCoreRateLimit;
using Azure.Core;
using Azure.Identity;
using FluentValidation;
using FluentValidation.AspNetCore;
using CustomerService.API.RateLimit;
using CustomerService.API.Validators;
using CustomerService.Common.Cache;
using CustomerService.Common.Models.Configuration;
using CustomerService.Common.Models.Constant;
using CustomerService.Common.Models.DTO;
using CustomerService.Common.Repository.Implementation;
using CustomerService.Common.Repository.Interfaces;
using CustomerService.Common.Repository.Wrapper;
using CustomerService.Common.Security;
using CustomerService.Common.Services.Implementation;
using CustomerService.Common.Services.Interfaces;
using CustomerService.EF;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CustomerService.API
{
    public class Startup
    {
        private const string MicroserviceName = "Customer Microservice";
        private const string ComponentName = "Customer API";
        private const string Environment = "Environment";

        private readonly string AllowSpecificOriginsPolicy = "AllowSpecificOriginsPolicy";

        private static bool IsLocal => string.Equals(_environment, "LOCAL", StringComparison.OrdinalIgnoreCase);

        private static string _environment;
        private static SecretClient _secretClient;
        private static IWebHostEnvironment _webHostEnvironment;
        private static IServiceCollection _serviceCollection;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _environment = GetEnvironmentVariable("Environment") ?? Configuration["Environment"];
            _secretClient = SetupConfiguration("KEYVAULT_ENDPOINT");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint behind Application Gateway.
            app.EnableApplicationGateway("/customer");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }

            var clientPolicyStore = app.ApplicationServices.GetRequiredService<IClientPolicyStore>(); 
            clientPolicyStore.SeedAsync().GetAwaiter().GetResult();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.EnableApplicationGatewayFriendlySwagger(ComponentName, _environment, GetEnvironmentVariable("SwaggerUIClientID"), GetEnvironmentVariable("APIAudience"));
            app.UseClientRateLimiting();
            app.UseRouting();
            app.UseCors(AllowSpecificOriginsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.AddApplicationInsightsApiPayloadLogging();
            app.UseJwtMiddleware();
            app.UseDiagnosticsMiddleware();
            app.UseExceptionHandler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            _serviceCollection = services;
            services.AddApplicationInsightsApiLogging(GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY"),
                GetEnvironmentVariable("CloudRoleNameAPI"),
                MicroserviceName,
                ComponentName,
                true,
                true);

            services.Configure<LoggerFilterOptions>(x => x.AddFilter("Microsoft.EntityFrameworkCore", GetEnvironmentVariable("MinimumLogLevel").GetLogLevel()));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            if (!IsLocal)
            {
                services.AddDbContext<CustomerDbContext>(options =>
                {
                    options.UseSqlServer(GetEnvironmentVariable("SQLConnectionStringTemplate"),
                        sqlOptions =>
                        {
                            int maxRetryCount = GetEnvironmentVariable("SQLConnectionMaxRetryCount").ParseInt(5);
                            int maxRetryDelayInSeconds = GetEnvironmentVariable("SQLConnectionRetryDelayInSeconds").ParseInt(30);

                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount,
                                TimeSpan.FromSeconds(maxRetryDelayInSeconds),
                                errorNumbersToAdd: new List<int> { -2 });
                        });
                });
            }
            else
            {
                services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("InMemoryCustomerDB"));
            }

            services.AddScoped<IWrappedLogger, APIWrappedLogger>();
            services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            services.AddScoped<ISubscriptionFilterRepository, SubscriptionFilterRepository>();
            services.AddScoped<ICustomerAuthRepository, CustomerAuthRepository>();
            services.AddScoped<ISubscriberService, SubscriberService>();
            services.AddScoped<ISha256SignatureExecutor, Sha256SignatureExecutor>();
            services.AddScoped<IClientSecretRepository, ClientSecretRepository>();
            services.AddScoped<IClientRateLimitPolicyRuleRepository, ClientRateLimitPolicyRuleRepository>();
            services.AddScoped<IClientSecretWrapper, ClientSecretWrapper>();
            services.AddScoped<IHttpClientWrapper, HttpClientWrapper>();
            services.AddScoped<IDCSAHttpClientRepository, DCSAHttpClientRepository>();
            services.AddScoped<IJWTAccessTokenRepository, JWTAccessTokenRepository>();
            services.AddScoped<IOrderMilestonesService, OrderMilestonesService>();
            services.AddScoped<IMilestonesApiRepository, MilestonesApiRepository>();
            services.AddScoped<IExternalBookingSubscriptionService, ExternalBookingSubscriptionService>();
            services.AddScoped<IMemoryCacheWrapper, MemoryCacheWrapper>();
            services.AddScoped<IEquipmentPackService, EquipmentPackService>();
            services.AddSingleton((p) => new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(120)));
            services.AddScoped<IAuthIdentityClientFactory, AuthIdentityClientFactory>();
            services.AddScoped<IMilestoneSentEventPublisher, MilestoneSentEventPublisher>(sp => new MilestoneSentEventPublisher(
                GetEnvironmentVariable("ProcessingTimeFunctionUrl"),
                sp.GetRequiredService<IHttpClientFactory>()));

            services.AddSingleton(x => new KeyVaultSetting { SecretClient = SetupConfiguration("KEYVAULT_RW_ENDPOINT") });

            services.AddSingleton(_ => new MilestoneAPIClientOptions(
                GetEnvironmentVariable("APIAuth0ClientId"),
                GetEnvironmentVariable("Auth0Authority"),
                GetEnvironmentVariable("MilestoneAuth0Audience"),
                GetKeyVaultSecret("APIAUTH0APPSECRET")
            ));

            services.AddMemoryCache();


            services.AddCors(opt =>
            {
                opt.AddPolicy(name: AllowSpecificOriginsPolicy, builder =>
                {
                    builder.WithOrigins(GetEnvironmentVariable("APIOrigin"))
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            RegisterFluentValidation(services);

            ConfigureHttpClient(services);
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddMvcCore()
                .AddMvcOptions(options =>
                {
                    options.RespectBrowserAcceptHeader = true;
                    options.ReturnHttpNotAcceptable = true;
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressMapClientErrors = true;
                })
                .AddControllersAsServices()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddApiExplorer();

            RegisterAuthentication();
            RegisterSwagger();

            IAuthGroupConfig authGroupConfig = new AuthGroupConfig
            {
                ApplicationRoles = new List<ApplicationRole>
                {
                    new ApplicationRole
                    {
                        ApplicationRoleName = ClientAppRoles.M2M,
                        PrincipalType = PrincipalType.M2M
                    },
                    new ApplicationRole
                    {
                        ApplicationRoleName = ClientAppRoles.DigitalTeam,
                        PrincipalType = PrincipalType.KOT_STAFF,
                        RoleBinding = ClientAppRoles.DigitalTeam
                    },
                },
                UseGraphApi = false
            };

            services.AddJwtMiddleware(authGroupConfig);
            services.AddSingleton<IClaimHelper, ClaimHelper>();
            services.AddTransient<IUserResolver, UserResolver>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthIdentityClientFactory, AuthIdentityClientFactory>();

            var auth0ClientSecret = GetKeyVaultSecret("APIAUTH0APPSECRET");
            RegisterOrderMilestoneApiClient(auth0ClientSecret);
            RegisterRateLimit();

            if (!IsLocal)
            {
                services.AddServiceRegistry(GetEnvironmentVariable("ServiceRegistryURI"),
                        GetEnvironmentVariable("Auth0Authority"),
                        GetEnvironmentVariable("ServiceRegistryAudience"),
                        GetEnvironmentVariable("APIAuth0ClientId"),
                        auth0ClientSecret,
                        MicroserviceName,
                        ComponentName);
            }
        }

        private static void RegisterFluentValidation(IServiceCollection services)
        {
            services.AddFluentValidation();
            services.AddFluentValidationRulesToSwagger();
            services.AddTransient<IValidator<EquipmentPackDTO>, EquipmentPackValidator>();
            services.AddTransient<IValidator<GetOrderMilestonesRequestDTO>, GetOrderMilestonesRequestValidator>();
            services.AddTransient<IValidator<ExternalBookingSubscriptionDTO>, ExternalBookingSubscriptionValidator>();
        }

        private string GetKeyVaultSecret(string secretName)
        {
            if (IsLocal)
                return "noToken";

            var keyVaultSecret = _secretClient.GetKeyVaultSecret(secretName);

            if (string.IsNullOrEmpty(keyVaultSecret))
            {
                throw new Exception($"{secretName} secret value not set");
            }

            return keyVaultSecret;
        }

        private static void ConfigureHttpClient(IServiceCollection services)
        {
            var retrypolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TaskCanceledException>()
                .Or<TimeoutException>()
                .OrResult(x => x.StatusCode == System.Net.HttpStatusCode.Forbidden)
                .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(40), TimeSpan.FromSeconds(60) });

            services.AddHttpClient(HttpClientConstant.HttpClient, client => client.Timeout = TimeSpan.FromSeconds(120))
              .SetHandlerLifetime(TimeSpan.FromMinutes(30))
              .AddPolicyHandler(retrypolicy);
        }

        private static SecretClient SetupConfiguration(string keyVaultEndpoint)
        {
            var keyVaultUri = new Uri($"{GetEnvironmentVariable(keyVaultEndpoint)}");

            TokenCredential tokenCredential = new VisualStudioCredential(new VisualStudioCredentialOptions());

            if (!IsLocal)
            {
                var defaultAzureCredentialOptions = new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = GetEnvironmentVariable("ManagedIdentityClientId")
                };

                tokenCredential = new DefaultAzureCredential(defaultAzureCredentialOptions);
            }

            return new SecretClient(keyVaultUri, tokenCredential);
        }


        private static void RegisterSwagger()
        {
            // Register the Swagger generator, defining one or more Swagger documents
            _serviceCollection.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerService.API", Version = "v1", Description = "CustomerService.API" });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "CustomerService.API.xml");
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{GetEnvironmentVariable("Auth0Tenant")}/authorize")
                        }
                    },
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });

                c.OperationFilter<AuthOperationFilter>();
            });
        }

        private void RegisterRateLimit()
        {
            // see all details here: https://github.com/stefanprodan/AspNetCoreRateLimit/wiki/IpRateLimitMiddleware#defining-rate-limit-rules
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var ratePolicyRepo = serviceProvider.GetRequiredService<IClientRateLimitPolicyRuleRepository>();
            ClientRateLimitPolicies policies = ratePolicyRepo.GetAll().GetAwaiter().GetResult();

            _serviceCollection.AddOptions();
            _serviceCollection.AddMemoryCache();
            _serviceCollection.Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"));
            _serviceCollection.Configure<ClientRateLimitPolicies>(option => { option.ClientRules = policies.ClientRules; });
            _serviceCollection.AddInMemoryRateLimiting();
            _serviceCollection.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        private static void RegisterAuthentication()
        {
            var authority = $"{GetEnvironmentVariable("Auth0Tenant")}/";
            var audience = GetEnvironmentVariable("APIAudience");

            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                GetEnvironmentVariable("Auth0WellKnownKeysLocation"),
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var discoveryDocument = configurationManager.GetConfigurationAsync().GetAwaiter().GetResult();
            var signingKeys = discoveryDocument.SigningKeys;

            _serviceCollection.AddAuth0AADAuthentication(audience, authority, authority, signingKeys, null, null);
            _serviceCollection.AddAuth0AADAuthorizationPolicy();
        }

        private static string GetEnvironmentVariable(string environmentVariableName)
        {
            return environmentVariableName.GetEnvironmentVariable();
        }

        private static void RegisterOrderMilestoneApiClient(string auth0ClientSecret)
        {
            var apiAuthenticationOptions = AuthenticationOptionsBuilder
                  .ForAuth0()
                  .WithAudience(GetEnvironmentVariable("MilestoneAuth0Audience"))
                  .WithClientId(GetEnvironmentVariable("APIAuth0ClientId"))
                  .WithClientSecret(auth0ClientSecret)
                  .WithAuthority(GetEnvironmentVariable("Auth0Authority"))
                  .Build();

            var orderMilestonesClientOptions = new MilestonesApiClientOptions(GetEnvironmentVariable("MilestoneApiBaseAddress"),
                TimeSpan.FromSeconds(20),
                apiAuthenticationOptions);

            _serviceCollection.AddScoped<IMilestonesApiClient, MilestonesApiClient>(sp =>
            {
                var logger = sp.GetService<IWrappedLogger>();
                var httpClientFactory = sp.GetService<IHttpClientFactory>();
                var correlationTokenProvider = sp.GetService<ICorrelationTokenProvider>();
                var memoryCache = sp.GetService<IMemoryCache>();
                var authClientFactory = sp.GetService<IAuthIdentityClientFactory>();
                var authClient = authClientFactory.CreateAuthClient(logger, AuthIdentityClientFactory.Auth0AuthClient, httpClientFactory, memoryCache);

                return new MilestonesApiClient(logger, httpClientFactory, authClient, correlationTokenProvider, orderMilestonesClientOptions);
            });
        }
    }
}
