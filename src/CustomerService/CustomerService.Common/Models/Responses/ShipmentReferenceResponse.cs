using Newtonsoft.Json;

namespace CustomerService.Common.Models
{
    public class ShipmentReferenceResponse
    {
        public string BookingReference { get; set; }

        public string CustomerCode { get; set; }

        public string OrderNumber { get; set; }

        [JsonProperty("SCAC")]
        public string SCAC { get; set; }
    }
}