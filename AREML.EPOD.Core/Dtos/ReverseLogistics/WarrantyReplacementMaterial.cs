using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class WarrantyReplacementMaterial
    {
        public string MATERIAL_CODE { get; set; }
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public string INV_NO { get; set; }
    }
}
