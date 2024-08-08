using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class ForgotPassword
    {
        public Guid UserID { get; set; }
        public string EmailAddress { get; set; }
        public string NewPassword { get; set; }
        public string Token { get; set; }
    }
}
