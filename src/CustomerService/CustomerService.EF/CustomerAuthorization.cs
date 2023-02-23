using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CustomerService.EF
{
    public partial class CustomerAuthorization
    {
        public CustomerAuthorization()
        {
            ClientRateLimitPolicyRules = new HashSet<ClientRateLimitPolicyRule>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDateUTC { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime LastUpdatedDateUTC { get; set; } = DateTime.UtcNow;

        public ICollection<ClientRateLimitPolicyRule> ClientRateLimitPolicyRules { get; set; }
    }
}