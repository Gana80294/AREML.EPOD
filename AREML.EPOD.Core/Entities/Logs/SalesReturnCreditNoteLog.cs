using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Entities.Logs
{
    public class SalesReturnCreditNoteLog
    {
        [Key]
        public int CRID { get; set; }
        public string InvoiceNumber { get; set; }

        public string CreditInvoice { get; set; }


        public string MaterialCode { get; set; }

        public double Qty { get; set; }


        public string ItemNumber { get; set; }

        public bool IsActive { get; set; }
    }
}
