using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class ForgotPasswordOTP
    {
        public string UserCode { get; set; }
        public string OTP { get; set; }
        public string NewPassword { get; set; }
    }

    public class changePassword
    {
        public string UserID { get; set; }
        public string UserName {get;set; }
        public string NewPassword { get; set; }
        public string CurrentPassword { get; set; }
    }
}
