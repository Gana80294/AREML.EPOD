using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class RolewithGroup
    {
        public string RoleName { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerGroupName { get; set; }

        public Guid RoleID { get; set; }
        public Guid UserID { get; set; }
    }
}
