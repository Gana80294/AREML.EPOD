using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InsertDeliveryDetails
    {
        public string vehicleNumber { get; set; }
        public string customerCode { get; set; }
        public string foNumber { get; set; }
        public long totalTravelTime { get; set; }
        public decimal totalDistance { get; set; }
        public string deliveryDate { get; set; }
        public string deliveryTime { get; set; }
    }
}
