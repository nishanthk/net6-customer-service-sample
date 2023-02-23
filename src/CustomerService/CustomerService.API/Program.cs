using CustomerService.Common.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CustomerService.API
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureLogging(logging =>
                {
                    logging.AddConsole();

                    logging.SetMinimumLevel(LoggingHelper.GetLogLevel(Environment.GetEnvironmentVariable("MinimumLogLevel")))
                    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

                })
                .UseStartup<Startup>();
            });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
    }
}