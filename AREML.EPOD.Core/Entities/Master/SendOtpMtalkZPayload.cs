using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class SendOtpMtalkZPayload
    {
        public string apikey { get; set; }
        public string senderid { get; set; }
        public string number { get; set; }
        public string message { get; set; }
        public string format { get; set; }
    }
}
