using System;

namespace CustomerService.Common.Models.DTO
{
    public class EquipmentPackDTO
    {
        public string OrderNumber { get; set; }
        public string BookingReference { get; set; }
        public string CarrierSCAC { get; set; }
        public string CustomerCode { get; set; }
        public string EquipmentReference { get; set; }
        public string ISOEquipmentGroupCode { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string PurposeCode { get; set; }
    }
}
