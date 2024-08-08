using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Logs
{
    public class UserActionHistory
    {
        [Key]
        public int LogID { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public int TransID { get; set; }
        public string Action { get; set; }
        public string ChangesDetected { get; set; }
        public DateTime DateTime { get; set; }
    }
}
