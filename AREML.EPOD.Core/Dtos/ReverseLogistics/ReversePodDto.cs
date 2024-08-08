using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodDto
    {
        public string DC_NUMBER { get; set; }
        public string DC_DATE { get; set; }
        public string INV_NO { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public string SLA_DATE { get; set; }
        public int PENDING_DAYS { get; set; } = 0;
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public List<ReversePodMaterialDto> MATERIALS { get; set; }
    }
}
