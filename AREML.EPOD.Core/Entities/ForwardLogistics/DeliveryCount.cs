using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class DeliveryCount
    {
        public int TotalDelivery { get; set; }
        public int InLineDelivery { get; set; }
        public int DelayedDelivery { get; set; }
    }
}
