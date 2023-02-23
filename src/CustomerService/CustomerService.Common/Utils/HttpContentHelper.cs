using API.SDK.Models;

namespace CustomerService.Common.Utils
{
    public static class HttpContentHelper
    {
        public static string TryGetErrorDetails(string httpContent)
        {
            if (string.IsNullOrWhiteSpace(httpContent)) return httpContent;

            if (JsonHelper.TryDeserializeJson(httpContent, out ProblemDetails ProblemDetails))
            {
                return ProblemDetails.Detail;
            }

            return httpContent;
        }
    }
}
