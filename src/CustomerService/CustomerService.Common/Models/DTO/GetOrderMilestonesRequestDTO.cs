namespace CustomerService.Common.Models.DTO
{
    public class GetOrderMilestonesRequestDTO
    {
        public string CustomerCode { get; set; }
        public string OrderNumber { get; set; }
        public string EquipmentReference { get; set; }
        public string BookingReference { get; set; }
        public string CarrierSCAC { get; set; }
    }
}
