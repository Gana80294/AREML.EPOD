using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class RPOD_MATERIAL
    {
        [Key]
        public int MATERIAL_ID { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public string MATERIAL_CODE { get; set; }
        public double? QUANTITY { get; set; }
        public double? HAND_OVERED_QUANTITY { get; set; }
        public double? RECEIVED_QUANTITY { get; set; }
        [MaxLength(30)]
        public string STATUS { get; set; }
        public string REMARKS { get; set; }

        [ForeignKey("RPOD_HEADER_ID")]
        public virtual RPOD_HEADER REVERSE_POD { get; set; } = null;
    }
}
