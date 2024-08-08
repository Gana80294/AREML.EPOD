using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class Item
    {
        public string INV_NO { get; set; }
        public string HEADER_ID { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NO { get; set; }
        public string PLANT { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string ODIN { get; set; }
        public string POD_DATE { get; set; }
        public string POD_TIME { get; set; }
        public string PROPOSED_DELIVERY_DATE { get; set; }
        public string VEHICLE_REPORTED_DATE { get; set; }
        public string TRANS_DAMAGE_REMARKS { get; set; }
        public string ACTUAL_DELIVERY_DATE { get; set; }
        public double? DELIVERY_QTY { get; set; }
        public double? POD_QTY { get; set; }
        public string POD_EVENT_DATE { get; set; }
        public string POD_EVENT_TIME { get; set; }
        public string POD_DOC_UPLOAD_DATE { get; set; }
        public string POD_DOC_UPLOAD_TIME { get; set; }
    }
}
