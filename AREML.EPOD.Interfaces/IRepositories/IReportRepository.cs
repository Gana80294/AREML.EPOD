using AREML.EPOD.Core.Entities.ForwardLogistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IReportRepository
    {
        Task<List<string>> GetDivisions();
        Task<List<ReportInvoice>> GetPendingInvoiceDetails(Guid UserID);

        Task<List<ReportInvoice>> GetFilteredInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null);
        Task<List<ReportInvoice>> GetFilteredInvoiceDetails(Guid UserID, int CurrentPage, int Records, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null, string CustomerGroup = null);
        Task<List<ReportInvoice>> GetFilteredInvoiceDetails(FilterClass filterClass);
        Task<byte[]> DownloadInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null);

        Task<byte[]> DownloadInvoiceDetails(FilterClass filterClass);
        Task<byte[]> DownloadInvoiceDetailsForAutomation(FilterClass filterClass);

        Task<HttpResponseMessage> DowloadPODDocument(int HeaderID, int AttachmentID);
    }
}
