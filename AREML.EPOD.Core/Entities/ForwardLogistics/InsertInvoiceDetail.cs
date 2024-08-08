using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InsertInvoiceDetail
    {
        public int ID { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public string INV_NO { get; set; }
        public string ODIN { get; set; }
        public string INV_DATE { get; set; }
        public string INV_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string ORGANIZATION { get; set; }
        public string DIVISION { get; set; }
        public string VEHICLE_NO { get; set; }
        public string VEHICLE_CAPACITY { get; set; }
        public string EWAYBILL_NO { get; set; }
        public string EWAYBILL_DATE { get; set; }
        public string LR_NO { get; set; }
        public string LR_DATE { get; set; }
        public string FWD_AGENT { get; set; }
        public string CARRIER { get; set; }
        public string FREIGHT_ORDER { get; set; }
        public string FREIGHT_ORDER_DATE { get; set; }
        public string OUTBOUND_DELIVERY { get; set; }
        public string OUTBOUND_DELIVERY_DATE { get; set; }
        public string ACTUAL_DISPATCH_DATE { get; set; }
        public string PROPOSED_DELIVERY_DATE { get; set; }
        public string VEHICLE_REPORTED_DATE { get; set; }
        public string ACTUAL_DELIVERY_DATE { get; set; }
        public string TRANSIT_LEAD_TIME { get; set; }
        public string DISTANCE { get; set; }
        public string CANC_INV_STATUS { get; set; }
        public string STATUS { get; set; }
        public string STATUS_DESCRIPTION { get; set; }
        public bool ISXMLCREATED { get; set; }
        public DateTime? XMLMOVED_ON { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_ON { get; set; }
        public bool IS_ACTIVE { get; set; }

        public string CUSTOMER_GROUP { get; set; }
        public string CUSTOMER_GROUP_DESC { get; set; }

        public string SECTOR_DESCRIPTION { get; set; }

        public string CUSTOMER_DESTINATION { get; set; }

        public string PLANT_CODE { get; set; }

        public string GROSS_WEIGHT { get; set; }
        public string DRIVER_CONTACT { get; set; }
        public List<P_INV_ITEM_DETAIL> ITEM_LIST { get; set; }
    }
}
