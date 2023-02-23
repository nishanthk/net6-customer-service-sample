using Microsoft.Extensions.Logging;
using System;

namespace CustomerService.Common.Utils
{
    public static class LoggingHelper
    {
        public static LogLevel GetLogLevel(string minimumLogLevelFromConfig)
        {
            LogLevel minimumLogLevel = LogLevel.Warning;

            if (!string.IsNullOrWhiteSpace(minimumLogLevelFromConfig) && Enum.IsDefined(typeof(LogLevel), minimumLogLevelFromConfig.Trim()))
            {
                minimumLogLevel = (LogLevel)Enum.Parse(typeof(LogLevel), minimumLogLevelFromConfig.Trim(), true);
            }

            return minimumLogLevel;
        }
    }
}
