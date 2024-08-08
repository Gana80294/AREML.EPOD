using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Logs
{
    public class UserCreationErrorLog
    {
        [Key]
        public int LogID { get; set; }
        public DateTime Date { get; set; }

        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string ContactNo { get; set; }

        public string RoleName { get; set; }

        public string LogReson { get; set; }
    }
}
