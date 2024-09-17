using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class DocumentHistory
    {
        [Key]
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FilePath { get; set; }
        public bool IsProcessed { get; set; }
        public string Message { get; set; }
    }
}
