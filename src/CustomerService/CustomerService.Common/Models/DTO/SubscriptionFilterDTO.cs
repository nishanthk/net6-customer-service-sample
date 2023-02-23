using CustomerService.Common.Models.Enum;
using System;
using System.Collections.Generic;

namespace CustomerService.Common.Models.DTO
{
    public class SubscriptionFilterDTO
    {
        public int Id { get; set; }

        public int SubscriberId { get; set; }

        public string Name { get; set; }

        public List<SubscriptionRuleDTO> Rules { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDateUTC { get; set; }

        public DateTime LastUpdatedDateUTC { get; set; }
    }
}
