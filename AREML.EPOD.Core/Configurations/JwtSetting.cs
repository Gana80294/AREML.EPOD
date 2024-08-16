using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Configurations
{
    public class JwtSetting
    {
        public string securityKey { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
    }
}
