using CustomerService.Common.Models;
using CustomerService.Common.Models.DTO;
using CustomerService.Common.Models.Enum;
using MilestoneService.DTO.API.V1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Common.Services.Interfaces
{
    public interface ISubscriberService
    {
        Task<IList<SubscriberDTO>> GetSubscriber(string customerCode, Dictionary<string, string> messageHeaders);
        Task<SubscriberDTO> GetSubscriber(Guid subscriptionId);

        Task<IEnumerable<OrderMilestoneDTO>> ApplySubscriptionFiltering(IList<OrderMilestoneDTO> milestoneDTOs, string customerCode);
    }
}