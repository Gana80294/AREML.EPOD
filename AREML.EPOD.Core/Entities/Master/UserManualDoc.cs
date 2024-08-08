using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class UserManualDoc
    {
        [Key]
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
