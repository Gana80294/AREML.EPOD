using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class OTPResponseBody
    {
        public Guid UserGuid { get; set; }
        public string OTPtranID { get; set; }
    }
}
