using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class SendOTPResponse
    {
        public string req_id { get; set; }
        public string status { get; set; }
        public string submittedAt { get; set; }
    }
}
