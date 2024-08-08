using AREML.EPOD.Core.Entities.ReverseLogistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodDetails
    {
        public int RPOD_HEADER_ID { get; set; }
        public string DC_NUMBER { get; set; }
        public DateTime? DC_DATE { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        public string STATUS { get; set; }
        public DateTime? SLA_DATE { get; set; }
        public int PENDING_DAYS { get; set; }
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public List<RPOD_LR_DETAIL> LR_DETAILS { get; set; } = new List<RPOD_LR_DETAIL>();
    }
}
