using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class SMSOTPChangePasswordHistory
    {
        [Key]
        public string OTPID { get; set; }
        public string OTP { get; set; }

        public DateTime OTPCreatedOn { get; set; }

        public DateTime OTPExpiredOn { get; set; }

        public DateTime? OTPUsedOn { get; set; }

        public bool IsOTPUSed { get; set; }

        public string MobileNumber { get; set; }

        public string UserName { get; set; }

        public bool IsPasswordChanged { get; set; }
    }
}
