using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodUpdation
    {
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public int Code { get; set; }
        public DateTime? DC_RECEIEVED_DATE { get; set; }
        public DateTime? DC_ACKNOWLEDGEMENT_DATE { get; set; }
        public string STATUS { get; set; }
        public List<ReversePodItemUpdation> MATERIALS { get; set; }
    }
}
