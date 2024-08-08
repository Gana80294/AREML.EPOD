using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class InvoiceStatusCount
    {
        public int TotalInvoices { get; set; }
        public int ConfirmedInvoices { get; set; }
        public int PartiallyConfirmedInvoices { get; set; }
        public int SavedInvoices { get; set; }
        public int PendingInvoices { get; set; }
    }
}
