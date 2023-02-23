using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CustomerService.Common.Extensions
{
    public static class JsonExtension
    {
        public static string SerializeToJsonWithCamelCase<T>(this T message) 
        {
            var option = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                    {
                        OverrideSpecifiedNames = false
                    }
                }
            };

            return JsonConvert.SerializeObject(message, option);
        }
    }
}
