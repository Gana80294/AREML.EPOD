using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class MobileVersion
    {
        [Key]
        public int Id { get; set; }
        public string VersionCode { get; set; }
    }
}
