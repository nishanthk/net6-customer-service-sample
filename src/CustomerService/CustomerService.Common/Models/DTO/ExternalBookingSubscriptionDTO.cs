namespace CustomerService.Common.Models.DTO
{
    public class ExternalBookingSubscriptionDTO
    {
        public string OrderNumber { get; set; }

        public string CustomerCode { get; set; }

        public string BookingReference { get; set; }

        public string CarrierCode { get; set; }
    }
}
