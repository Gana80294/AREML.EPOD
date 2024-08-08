using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AREML.EPOD.Core.Entities.Mappings
{
    [PrimaryKey("SGID","CGID")]
    public class SLSGroupWithCustomerGroupMap
    {
        public int SGID { get; set; }

        public int CGID { get; set; }

        public string SLSGroupCode { get; set; }
        public string CustomerGroupCode { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}
