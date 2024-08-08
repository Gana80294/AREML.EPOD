using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class P_INV_ITEM_DETAIL
    {
        [Key]
        public int ITEM_ID { get; set; }
        public string ITEM_NO { get; set; }
        public int HEADER_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public string MATERIAL_DESCRIPTION { get; set; }
        public double? QUANTITY { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        public string QUANTITY_UOM { get; set; }
        public string STATUS { get; set; }
        public string STATUS_DESCRIPTION { get; set; }
        public string REASON { get; set; }
        public string REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_ON { get; set; }
        public bool IS_ACTIVE { get; set; }
        public string ITEM_WEIGHT { get; set; }
    }
}
