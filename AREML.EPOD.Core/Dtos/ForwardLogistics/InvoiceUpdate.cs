using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ForwardLogistics
{
    public class InvoiceUpdate
    {
        [Required]
        public DateTime VEHICLE_REPORTED_DATE { get; set; }
        [Required]
        public int HEADER_ID { get; set; }
    }
}
