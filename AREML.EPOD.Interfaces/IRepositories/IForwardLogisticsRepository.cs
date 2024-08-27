using AREML.EPOD.Core.Dtos.ForwardLogistics;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IForwardLogisticsRepository
    {

        Task<List<Invoice_Header_View>> GetAllInvoiceDetails(Guid UserID);
        Task<List<P_INV_HEADER_DETAIL>> GetAllInvoiceDetailByUser(string UserCode);
        Task<List<Invoice_Header_View>> GetOpenAndSavedInvoiceDetailByUser(string UserCode);
        Task<List<Invoice_Header_View>> FilterInvoiceDetailByUser(FilterClass filterClass);

        Task<HttpResponseMessage> DownloadInvoiceDetailByUser(FilterClass filterClass);
        Task<List<Invoice_Header_View>> GetConfirmedInvoiceDetails(Guid UserID);

        Task<bool> ApproveSelectedInvoices(ApproverDetails approverDetails);

        Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByHeaderID(int HeaderID);
        Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByID(Guid UserID, int ID);
        Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByUserAndID(string UserCode, int ID);

        Task<P_INV_HEADER_DETAIL> UpdateInvoiceItems(InvoiceUpdation invoiceUpdation);
        Task<bool> ConfirmInvoiceItems(InvoiceUpdation1 invoiceUpdation);
        Task<AttachmentStatus> GetAttachmentID(int headerId);
        Task<bool> CreateUserActionHistory(UserActionHistory log);
        Task<List<UserActionHistoryView>> GetUserActionHistories(ActionHistoryFilter filter);
        Task<HttpResponseMessage> DownloadActionHistoryLog(ActionHistoryFilter filter);
        Task<bool> AddInvoiceAttachment();
        Task<List<P_INV_HEADER_DETAIL>> GetAllSavedInvoicesByUserID(Guid UserID);
        Task<List<P_INV_HEADER_DETAIL>> GetAllSavedInvoices();
        Task<List<InvoiceHeaderDetails>> FilterSavedInvoicesByUserID(FilterClass filterClass);
        Task<byte[]> DownloadSavedInvoicesByUserID(FilterClass filterClass);
        Task<List<P_INV_HEADER_DETAIL>> GetAllPartiallyConfirmedInvoices();

        Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoices(int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null);

        Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoices(FilterClass filterClass);
        Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoicesForAdmin(FilterClass filterClass);
        Task<byte[]> DownloadPartiallyConfirmedInvoices(FilterClass filterClass);
        Task<bool> UpdatePartiallyConfirmedInvoiceStatus(int HeaderID, string Status);
        Task<bool> UpdateVehicleUnloadedDateByLR(LRWithVehicleUnloaded request);
        Task<bool> UpdateLRDateByInvoiceNo(LRDateUpdate req);
        Task<bool> UpdateLRNumberByInvoiceNo(LRNumberUpdate req);
        Task<bool> SaveScrollNotification(ScrollNotification notification);
        Task<List<ScrollNotification>> GetScrollNotification();
        Task<List<DocumentHistoryView>> GetDocumentHistoryById(string invoiceNumber);
        Task<byte[]> DowloandHistoryDocument(int id);

        #region SAP Integration
        Task<InsertInvoiceResponse> InsertInvoiceDetails(InsertInvoiceDetail insertInvoiceDetail);
        #endregion

        #region Acknowledgement
        Task<ResponseMessage> ConfirmInvoice(InvoiceUpdate invoiceUpdate, byte[] fileBytes);
        Task<ResponseMessage> ConfirmInvoiceDetails(InvoiceUpdate invoiceUpdate, byte[] fileBytes);
        #endregion

        #region Sales Return
        Task<string> SalesReturns(SalesReturnProps salesReturnProps);
        Task<string> CancelSalesReturn(string CRinvoiceno);
        Task<string> InvoiceCancellation(string Invoiceno);
        #endregion
    }
}
