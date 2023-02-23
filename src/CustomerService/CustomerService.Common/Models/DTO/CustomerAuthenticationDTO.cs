using CustomerService.Common.Models.Enum;
using System;

namespace CustomerService.Common.Models.DTO
{
    public class CustomerAuthenticationDTO
    {
        public Guid Id { get; set; }

        public string ClientId { get; set; }

        public string UserId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDateUTC { get; set; }

        public DateTime LastUpdatedDateUTC { get; set; }
    }
}
