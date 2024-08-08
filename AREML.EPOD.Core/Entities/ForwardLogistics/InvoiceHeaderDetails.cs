using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InvoiceHeaderDetails
    {
        public int HEADER_ID { get; set; }
        public string ORGANIZATION { get; set; }
        public string DIVISION { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public string INV_NO { get; set; }
        public string ODIN { get; set; }
        public DateTime? INV_DATE { get; set; }
        public string INV_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string VEHICLE_NO { get; set; }
        public string VEHICLE_CAPACITY { get; set; }
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public DateTime? PROPOSED_DELIVERY_DATE { get; set; }
        public DateTime? VEHICLE_REPORTED_DATE { get; set; }
        public DateTime? ACTUAL_DELIVERY_DATE { get; set; }
        public string STATUS { get; set; }
        public int ATTACHMENT_ID { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string FWD_AGENT { get; set; }
        public string EWAYBILL_NO { get; set; }
        public double? INVOICE_QUANTITY { get; set; }
        public string DRIVER_CONTACT { get; set; }
        public string TRACKING_LINK { get; set; }
        public decimal TOTAL_TRAVEL_TIME { get; set; }
        public decimal TOTAL_DISTANCE { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string DELIVERY_TIME { get; set; }
    }
}
