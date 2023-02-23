using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace CustomerService.API.UnitTests.Lib
{
    public static class Extensions
    {
        public static void ReplaceWithMock<IServiceType>(this IServiceCollection services, IServiceType mockService)
           where IServiceType : class
        {
            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IServiceType));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddScoped<IServiceType>(s => mockService);

            return;
        }
    }

}