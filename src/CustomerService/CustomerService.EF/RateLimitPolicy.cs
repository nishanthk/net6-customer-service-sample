using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.EF
{
    public class RateLimitPolicy
    {
        public RateLimitPolicy()
        {
            ClientRateLimitPolicyRules = new HashSet<ClientRateLimitPolicyRule>();
        }

        [Key]
        public int Id { get; set; }

        public string Endpoint { get; set; } = "*";

        public string Period { get; set; } = "20s";

        public int Limit { get; set; } = 100;

        [Required]
        public DateTime CreatedDateUTC { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedDateUTC { get; set; } = DateTime.UtcNow;

        public ICollection<ClientRateLimitPolicyRule> ClientRateLimitPolicyRules { get; set; }
    }
}
