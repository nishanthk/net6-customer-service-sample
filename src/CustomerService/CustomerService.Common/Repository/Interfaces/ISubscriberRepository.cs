using CustomerService.Common.Models.DTO;
using CustomerService.EF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Common.Repository.Interfaces
{
    public interface ISubscriberRepository
    {
        Task<Subscriber> GetAsync(Guid subscriptionId);
        Task<IList<Subscriber>> GetAsync(string customerCode);
        Task<int> UpsertAsync(SubscriberDTO subscriber);
    }
}