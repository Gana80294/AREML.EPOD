using AREML.EPOD.Core.Entities.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Core.Dtos.ReverseLogistics
{
    public class DeleteInvoiceDto
    {
        public string InvNo { get; set; }
        public UserActionHistory log { get; set; }
    }
}
