using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class ReversePodFilterClass
    {
        public Guid UserID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> Status { get; set; }
        public List<string> PlantList { get; set; }
        public List<string> PlantGroupList { get; set; }
        public string DcNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public int CurrentPage { get; set; }
        public int Records { get; set; }
    }
}
