using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Master
{
    public class SLSCustGroupData
    {
        public int SGID { get; set; }
        public string SLSGroupCode { get; set; }


        public List<int> CustomerGroupCode { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
