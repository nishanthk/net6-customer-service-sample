using CustomerService.Common.Models.Enum;
using System;

namespace CustomerService.Common.Models.DTO
{
    public class SubscriberDTO
    {
        public int Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public string CustomerCode { get; set; }

        public string UrlAddress { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDateUTC { get; set; }

        public DateTime LastUpdatedDateUTC { get; set; }
    }
}
