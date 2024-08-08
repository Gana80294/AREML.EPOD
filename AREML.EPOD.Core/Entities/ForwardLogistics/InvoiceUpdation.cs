using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InvoiceUpdation
    {
        public DateTime VEHICLE_REPORTED_DATE { get; set; }
        public List<P_INV_ITEM_DETAIL> InvoiceItems { get; set; }

    }
}
