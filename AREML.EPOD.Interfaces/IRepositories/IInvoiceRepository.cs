using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IInvoiceRepository
    {
        Task<List<P_INV_HEADER_DETAIL>> GetAllInvoiceDetails(Guid UserID);
        Task<bool> DeleteInvoices(DeleteInvoiceDto payload);
        Task<List<P_INV_HEADER_DETAIL>> GetInvoiceDetailByInvNo(string InvNo);
    }
}
