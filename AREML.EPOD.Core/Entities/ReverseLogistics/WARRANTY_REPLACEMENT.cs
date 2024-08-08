using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class WARRANTY_REPLACEMENT
    {
        [Key]
        public int Id { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public double? TOTAL_QUANTITY { get; set; }
        public double? BILLED_QUANTITY { get; set; }
        public double? BALANCE_QUANTITY { get; set; }
        public string INV_NO { get; set; }
        public double? SUPPLIED_QUANTITY { get; set; }

        [ForeignKey("RPOD_HEADER_ID")]
        public virtual RPOD_HEADER REVERSE_POD { get; set; } = null;
    }
}
