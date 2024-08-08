using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Mappings
{
    public class CFAUserMapping
    {
        [Key]
        public int ID { get; set; }
        public Guid CFAUserID { get; set; }
        public Guid UserID { get; set; }
    }
}
