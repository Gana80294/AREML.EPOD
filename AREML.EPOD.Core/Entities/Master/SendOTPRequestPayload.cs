﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class SendOTPRequestPayload
    {
        public int feedid { get; set; }
        public long username { get; set; }
        public string password { get; set; }
        public string jobname { get; set; }
        public long mobile { get; set; }
        public string messages { get; set; }
        public string templateid { get; set; }
        public string entityid { get; set; }
        public string senderid { get; set; }
    }
}
