using System;

namespace CustomerService.Common.Models.DTO
{
    public class SubscriptionRuleDTO
    {
        public string EventTypeCode { get; set; }
        public string CustomerCode { get; set; }
        public string EventClassifierCode { get; set; }
        public string FacilityTypeCode { get; set; }
        public string TransportMode { get; set; }

        public MessageTypeDto Message { get; set; }
    }

    public class MessageTypeDto
    {
        public string MessageType { get; set; }

        public string MessageVersion { get; set; }
    }
}
