using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class AffrimativeOTPBody
    {
        public Guid UserGuid { get; set; }

        public string newPassword { get; set; }

        public string recievedOTP { get; set; }

        public string OTPTransID { get; set; }
    }
}
