using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ReverseLogistics
{
    public class RPOD_LR_ATTACHMENT
    {
        [Key]
        public int Id { get; set; }
        public int RPOD_HEADER_ID { get; set; }
        public int Code { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
