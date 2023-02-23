using Newtonsoft.Json;

namespace CustomerService.Common.Models
{
    public class TransportResponse
    {
        public string VoyageNumber { get; set; }

        public string VesselIMONumber { get; set; }

        public string VesselName { get; set; }

        public string VesselFlag { get; set; }

        public string VesselCallSignNumber { get; set; }

        public string VesselOperatorCarrierCode { get; set; }

        public string VesselOperatorCarrierCodeProvider { get; set; }
    }
}