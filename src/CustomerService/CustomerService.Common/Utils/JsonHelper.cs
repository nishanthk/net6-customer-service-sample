using Newtonsoft.Json;

namespace CustomerService.Common.Utils
{
    public static class JsonHelper
    {
        public static bool TryDeserializeJson<T>(string jsonString, out T result) 
        {
            bool success = true;
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
            };
            result = JsonConvert.DeserializeObject<T>(jsonString, settings);
            return success;
        }
    }
}
