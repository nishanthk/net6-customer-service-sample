using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.EF
{
    public partial class SubscriptionFilter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SubscriberId { get; set; }

        public string Name { get; set; }

        [Required]
        public string Rules { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public FilterSourceEnum FilterSource { get; set; } = FilterSourceEnum.Webhook;

        [Required]
        public DateTime CreatedDateUTC { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedDateUTC { get; set; } = DateTime.UtcNow;

        public virtual Subscriber Subscriber { get; set; }
    }

    [System.Flags]
    public enum FilterSourceEnum
    {
        Webhook = 1,
        Api = 2
    }
}