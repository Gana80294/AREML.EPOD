using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class LRWithVehicleUnloaded
    {
        public string LRNumber { get; set; }
        public DateTime LRDate { get; set; }
        public DateTime VehicleUnloadedDate { get; set; }
        public string Customer { get; set; }
    }
}
