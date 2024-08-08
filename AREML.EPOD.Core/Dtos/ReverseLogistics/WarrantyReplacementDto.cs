using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class WarrantyReplacementDto
    {
        public string DC_NUMBER { get; set; }
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public List<WarrantyReplacementMaterial> MATERIALS { get; set; }
    }
}
