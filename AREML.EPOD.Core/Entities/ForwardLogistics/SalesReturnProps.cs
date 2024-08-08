using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.ForwardLogistics
{
    public class SalesReturnProps
    {
        public string InvoiceNumber { get; set; }
        public string CreditInvoice { get; set; }
        public List<SalesReturnMaterial> SalesReturnMaterial { get; set; }
    }
}
