using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class UpdateReverseLogistics
    {
        public string DC_NUMBER { get; set; }
        public double? HAND_OVERED_QUANTITY { get; set; }
        public string LR_NO { get; set; }
        public DateTime? LR_DATE { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        public DateTime? DC_RECEIEVED_DATE { get; set; }
    }
}
