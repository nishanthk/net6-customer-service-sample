using Common.SharedAppInterfaces;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CustomerService.Common.Cache
{
    public class MemoryCacheWrapper : IMemoryCacheWrapper
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;
        private readonly IWrappedLogger _logger;

        public MemoryCacheWrapper(IMemoryCache cache, MemoryCacheEntryOptions memoryCacheEntryOptions, IWrappedLogger logger)
        {
            _cache = cache;
            _memoryCacheEntryOptions = memoryCacheEntryOptions;
            _logger = logger;
        }

        public async Task<T> GetOrCreate<T>(string key, string target, Func<Task<T>> process)
        {
            if (!_cache.TryGetValue(key, out T value))
            {
                value = await process();
                _cache.Set(key, value, _memoryCacheEntryOptions);
            }
            else
            {
                var telemetryDependency = new DependencyTelemetry
                {
                    Success = true,
                    Target = target,
                    Type = "Cache",
                    Timestamp = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    ResultCode = "Success",
                    Data = JsonConvert.SerializeObject(value)
                };
                telemetryDependency.Properties.Add("Key", JsonConvert.SerializeObject(key));
                _logger.TrackTelemetryDependency(telemetryDependency);
            }

            return value;
        }
    }
}
