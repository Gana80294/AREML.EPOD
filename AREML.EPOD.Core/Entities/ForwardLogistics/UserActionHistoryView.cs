using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class UserActionHistoryView
    {
        public string InvoiceNumber { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public int TransID { get; set; }
        public string Action { get; set; }
        public string ChangesDetected { get; set; }
        public DateTime DateTime { get; set; }
    }
}
