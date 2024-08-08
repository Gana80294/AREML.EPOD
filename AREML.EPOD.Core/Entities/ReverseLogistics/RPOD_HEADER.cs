using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class RPOD_HEADER
    {
        [Key]
        public int RPOD_HEADER_ID { get; set; }
        [MaxLength(100)]
        public string DC_NUMBER { get; set; }
        public DateTime? DC_DATE { get; set; }
        public string CLAIM_TYPE { get; set; }
        public string CUSTOMER { get; set; }
        public string CUSTOMER_NAME { get; set; }
        public string PLANT { get; set; }
        public string PLANT_NAME { get; set; }
        [MaxLength(30)]
        public string STATUS { get; set; }
        public DateTime? SLA_DATE { get; set; }
        public int PENDING_DAYS { get; set; } = 0;
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
