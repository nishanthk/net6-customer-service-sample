using API.SDK.Models;
using Common.SharedAppInterfaces.Exceptions;
using CustomerService.Common.Utils;
using HTTP.Clients.Models;

namespace CustomerService.Common.Extensions
{
    public static class ApiResponseExtension
    {
        public static string GetErrorDetailsFromApiResponse<T>(this ApiResponse<T> response)
        {
            if (string.IsNullOrWhiteSpace(response.HttpContent)) return response.HttpContent;

            if (JsonHelper.TryDeserializeJson(response.HttpContent, out ProblemDetails ProblemDetails))
            {
                return ProblemDetails.Detail;
            }

            return response.HttpContent;
        }

        public static T HandleResponse<T>(this ApiResponse<T> response, string message)
        {
            if (response == null ||
                (response.IsExceptionResponse() && response.StatusCode == System.Net.HttpStatusCode.NotFound))
            {
                throw new NotFoundException();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new BusinessException(response.GetErrorDetailsFromApiResponse());
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new ForbiddenException($"The request is not authorized: {message}'");
            }

            if (response.IsSuccessResponse())
                return response.Data;

            throw new CriticalException($"An issue occurred when sending http request: {message}", skipAutomaticRetries: true);
        }
    }
}
