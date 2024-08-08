using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class ExportUserModel
    {
        public Guid UserID { get; set; }
        public string UserCode { get; set; }

        public string UserName { get; set; }

        public string EmailID { get; set; }

        public string Mobile { get; set; }

        public string RoleName { get; set; }

        public string Organization { get; set; }

        public string Plant { get; set; }

        public string SalesGroups { get; set; }

        public bool isAmuser { get; set; }
    }
}
