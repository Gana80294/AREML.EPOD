
using AREML.EPOD.Core.Entities.ReverseLogistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodMaterialDetails
    {
        public int MATERIAL_ID { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public double? HAND_OVERED_QUANTITY { get; set; }
        public double? CUSTOMER_PENDING_QUANTITY { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        public double? DC_PENDING_QUANTITY { get; set; }
        public string STATUS { get; set; }
        public string REMARKS { get; set; }
        public List<WARRANTY_REPLACEMENT> WARRANTY_REPLACEMENT_DETAILS { get; set; }
    }
}
