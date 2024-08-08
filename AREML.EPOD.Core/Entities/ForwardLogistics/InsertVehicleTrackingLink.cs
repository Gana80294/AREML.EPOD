using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InsertVehicleTrackingLink
    {
        public string vehicleNumber { get; set; }
        public string customerCode { get; set; }
        public string foNumber { get; set; }
        public string trackingLink { get; set; }

    }
}
