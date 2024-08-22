using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Configurations
{
    public class OtpSetting
    {
        public string otpRequestApi { get; set; }
        public string smsMsg { get; set; }
        public string apiKey { get; set; }
        public string senderID { get; set; }
        public string feedID { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string jobName { get; set; }
        public string templateID { get; set; }
        public string entityID { get; set; }
    }
}
