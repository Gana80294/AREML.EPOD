using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class ReportInvoice
    {
        public int HEADER_ID { get; set; }
        public string INV_NO { get; set; }
        public string ODIN { get; set; }
        public int ITEM_ID { get; set; }
        public string ITEM_NO { get; set; }
        public DateTime? INV_DATE { get; set; }
        public string INV_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string CUSTOMER_GROUP { get; set; }
        public string ORGANIZATION { get; set; }
        public string DIVISION { get; set; }
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public string FWD_AGENT { get; set; }
        public string CARRIER { get; set; }
        public string VEHICLE_CAPACITY { get; set; }
        public string EWAYBILL_NO { get; set; }
        public DateTime? EWAYBILL_DATE { get; set; }
        public string FREIGHT_ORDER { get; set; }
        public DateTime? FREIGHT_ORDER_DATE { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string MATERIAL_DESCRIPTION { get; set; }
        public double? QUANTITY { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        public string QUANTITY_UOM { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public string OUTBOUND_DELIVERY { get; set; }
        public DateTime? OUTBOUND_DELIVERY_DATE { get; set; }
        public DateTime? ACTUAL_DISPATCH_DATE { get; set; }
        public DateTime? PROPOSED_DELIVERY_DATE { get; set; }
        public DateTime? VEHICLE_REPORTED_DATE { get; set; }
        public DateTime? ACTUAL_DELIVERY_DATE { get; set; }
        public string POD_UPLOADE_STATUS { get; set; }
        public string TRANSIT_LEAD_TIME { get; set; }
        public string CANC_INV_STATUS { get; set; }
        public string STATUS { get; set; }
        public string DRIVER_CONTACT { get; set; }
        public string TRACKING_LINK { get; set; }
        public decimal TOTAL_TRAVEL_TIME { get; set; }
        public decimal TOTAL_DISTANCE { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string DELIVERY_TIME { get; set; }
        public int ATTACHMENT_ID { get; set; }
        public string ATTACHMENT_NAME { get; set; }
        public string CUSTOMER_GROUP_DESC { get; set; }
        public string SECTOR_DESCRIPTION { get; set; }
        public string CUSTOMER_DESTINATION { get; set; }
        public string PLANT_CODE { get; set; }
        public string GROSS_WEIGHT { get; set; }
        public string REMARKS { get; set; }
        public string DISTANCE { get; set; }
        public string ITEM_WEIGHT { get; set; }
    }
}
