using System;
using System.Threading.Tasks;

namespace CustomerService.Common.Cache
{
    public interface IMemoryCacheWrapper
    {
        Task<T> GetOrCreate<T>(string key, string target, Func<Task<T>> process);
    }
}