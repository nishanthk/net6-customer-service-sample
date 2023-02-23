using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.EF
{
    public partial class Subscriber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid SubscriptionId { get; set; }

        [Required]
        public string CustomerCode { get; set; }

        [Required]
        public string UrlAddress { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDateUTC { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedDateUTC { get; set; } = DateTime.UtcNow;

        public virtual ICollection<SubscriptionFilter> SubscriptionFilters { get; set; } = new HashSet<SubscriptionFilter>();
    }
}