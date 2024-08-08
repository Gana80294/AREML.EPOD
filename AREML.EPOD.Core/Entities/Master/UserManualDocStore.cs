using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class UserManualDocStore
    {
        [Key]
        public int Id { get; set; }
        public int UserManualDocId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string FileSize { get; set; }
        public byte[] FileContent { get; set; }
        [ForeignKey("UserManualDocId")]
        public virtual UserManualDoc UserManualDocs { get; set; }
    }
}
