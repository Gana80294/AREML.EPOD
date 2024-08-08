using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class FilterClass
    {
        public Guid UserID { get; set; }
        public string UserCode { get; set; }
        public int CurrentPage { get; set; }
        public int Records { get; set; }
        public List<string> Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string InvoiceNumber { get; set; }
        public List<string> Organization { get; set; }
        public List<string> Division { get; set; }
        public string Plant { get; set; }
        public List<string> PlantList { get; set; }
        public List<string> PlantGroupList { get; set; }
        public string CustomerName { get; set; }
        public List<string> CustomerGroup { get; set; }
        public string LRNumber { get; set; }
        public List<string> LeadTime { get; set; }
        public List<string> Delivery { get; set; }
    }
}
