using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.EF
{
    public class ClientRateLimitPolicyRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid CustomerAuthorizationId { get; set; }

        [Required]
        public int RateLimitPolicyId { get; set; }

        [Required]
        public DateTime CreatedDateUTC { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedDateUTC { get; set; } = DateTime.UtcNow;

        public CustomerAuthorization CustomerAuthorization { get; set; }

        public RateLimitPolicy RateLimitPolicy { get; set; }
    }
}
