using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class ReversePodItemUpdation
    {
        public int MATERIAL_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public int HAND_OVERED_QUANTITY { get; set; }
        public int RECEIVED_QUANTITY { get; set; }
        public string REMARKS { get; set; }
        public string STATUS { get; set; }
    }
}
