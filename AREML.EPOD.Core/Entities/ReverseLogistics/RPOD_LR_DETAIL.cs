using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class RPOD_LR_DETAIL
    {
        [Key]
        public int LR_ID { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public DateTime? DC_RECEIEVED_DATE { get; set; }
        public DateTime? DC_ACKNOWLEDGEMENT_DATE { get; set; }
        public int CUSTOMER_DOC_ID { get; set; }
        public int DC_DOC_ID { get; set; }
    }
}
