using AREML.EPOD.Core.Dtos.ForwardLogistics;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Interfaces.IRepositories;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ForwardLogisticsController : ControllerBase
    {
        private IForwardLogisticsRepository _forwardRepository;
        public ForwardLogisticsController(IForwardLogisticsRepository forwardRepository)
        {
            this._forwardRepository = forwardRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllInvoiceDetails(Guid UserID)
        {
            return Ok(await this._forwardRepository.GetAllInvoiceDetails(UserID));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoiceDetailByUser(string UserCode)
        {
            return Ok(await this._forwardRepository.GetAllInvoiceDetailByUser(UserCode));
        }

        [HttpGet]
        public async Task<IActionResult> GetOpenAndSavedInvoiceDetailByUser(string UserCode)
        {
            return Ok(await this._forwardRepository.GetOpenAndSavedInvoiceDetailByUser(UserCode));
        }
        [HttpPost]
        public async Task<IActionResult> FilterInvoiceDetailByUser(FilterClass filterClass)
        {
            return Ok(await this._forwardRepository.FilterInvoiceDetailByUser(filterClass));
        }


        [HttpPost]
        public async Task<IActionResult> DownloadInvoiceDetailByUser(FilterClass filterClass)
        {
            var fileContent = await this._forwardRepository.DownloadInvoiceDetailByUser(filterClass);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Invoice_Details_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetConfirmedInvoiceDetails(Guid UserID)
        {
            return Ok(await this._forwardRepository.GetConfirmedInvoiceDetails(UserID));
        }


        [HttpPost]
        public async Task<IActionResult> ApproveSelectedInvoices(ApproverDetails approverDetails)
        {
            return Ok(await this._forwardRepository.ApproveSelectedInvoices(approverDetails));
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoiceItemDetailsByHeaderID(int HeaderID)
        {
            return Ok(await this._forwardRepository.GetInvoiceItemDetailsByHeaderID(HeaderID));
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceItemDetailsByID(Guid UserID, int ID)
        {
            return Ok(await this._forwardRepository.GetInvoiceItemDetailsByID(UserID, ID));
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceItemDetailsByUserAndID(string UserCode, int ID)
        {
            return Ok(await _forwardRepository.GetInvoiceItemDetailsByUserAndID(UserCode, ID));
        }

        [HttpPost]

        public async Task<IActionResult> UpdateInvoiceItems(InvoiceUpdation invoiceUpdation)
        {
            return Ok(await this._forwardRepository.UpdateInvoiceItems(invoiceUpdation));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmInvoiceItems(InvoiceUpdation1 invoiceUpdation)
        {
            return Ok(await this._forwardRepository.ConfirmInvoiceItems(invoiceUpdation));
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachmentID(int headerId)
        {
            return Ok(await this._forwardRepository.GetAttachmentID(headerId));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserActionHistory(UserActionHistory log)
        {
            return Ok(await this._forwardRepository.CreateUserActionHistory(log));
        }

        [HttpPost]
        public async Task<IActionResult> GetUserActionHistories(ActionHistoryFilter filter)
        {
            return Ok(await this._forwardRepository.GetUserActionHistories(filter));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadActionHistoryLog(ActionHistoryFilter filter)
        {
            return Ok(await this._forwardRepository.DownloadActionHistoryLog(filter));
        }

        //[HttpPost]
        //public async Task<IActionResult> AddInvoiceAttachment()
        //{
        //    return Ok(await this._forwardRepository.AddInvoiceAttachment());
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllSavedInvoicesByUserID(Guid UserID)
        {
            return Ok(await this._forwardRepository.GetAllSavedInvoicesByUserID(UserID));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSavedInvoices()
        {
            return Ok(await this._forwardRepository.GetAllSavedInvoices());
        }

        [HttpPost]
        public async Task<IActionResult> FilterSavedInvoicesByUserID(FilterClass filterClass)
        {
            return Ok(await this._forwardRepository.FilterSavedInvoicesByUserID(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadSavedInvoicesByUserID(FilterClass filterClass)
        {
            var fileContent = await this._forwardRepository.DownloadSavedInvoicesByUserID(filterClass);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Saved Invoice_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPartiallyConfirmedInvoices()
        {
            return Ok(await this._forwardRepository.GetAllPartiallyConfirmedInvoices());
        }

        [HttpGet]

        public async Task<IActionResult> FilterPartiallyConfirmedInvoices(int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            return Ok(await this._forwardRepository.FilterPartiallyConfirmedInvoices(CurrentPage, Records, StartDate, EndDate, InvoiceNumber, Organization, Plant, CustomerName));
        }

        [HttpPost]
        public async Task<IActionResult> FilterPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            return Ok(await this._forwardRepository.FilterInvoiceDetailByUser(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterPartiallyConfirmedInvoicesForAdmin(FilterClass filterClass)
        {
            return Ok(await this._forwardRepository.FilterPartiallyConfirmedInvoicesForAdmin(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            var fileContent = await this._forwardRepository.DownloadPartiallyConfirmedInvoices(filterClass);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Partially_confirmed_Invoices_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePartiallyConfirmedInvoiceStatus(int HeaderID, string Status)
        {
            return Ok(await this._forwardRepository.UpdatePartiallyConfirmedInvoiceStatus(HeaderID, Status));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateVehicleUnloadedDateByLR(LRWithVehicleUnloaded request)
        {
            return Ok(await this._forwardRepository.UpdateVehicleUnloadedDateByLR(request));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLRDateByInvoiceNo(LRDateUpdate req)
        {
            return Ok(await this._forwardRepository.UpdateLRDateByInvoiceNo(req));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLRNumberByInvoiceNo(LRNumberUpdate req)
        {
            return Ok(await this._forwardRepository.UpdateLRNumberByInvoiceNo(req));
        }

        [HttpPost]
        public async Task<IActionResult> SaveScrollNotification(ScrollNotification notification)
        {
            return Ok(await _forwardRepository.SaveScrollNotification(notification));
        }

        [HttpGet]
        public async Task<IActionResult> GetScrollNotification()
        {
            return Ok(await this._forwardRepository.GetScrollNotification());
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentHistoryById(string invoiceNumber)
        {
            return Ok(await this._forwardRepository.GetDocumentHistoryById(invoiceNumber));
        }

        [HttpGet]
        public async Task<IActionResult> DowloandHistoryDocument(int id)
        {
            var fileContent = await this._forwardRepository.DowloandHistoryDocument(id);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Document_history_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPODDocument(int attachmentID)
        {
            return Ok(await _forwardRepository.DowloadPODDocument(attachmentID));
        }


        #region SAP integration
        [HttpPost]
        public async Task<IActionResult> InsertInvoiceDetails(InsertInvoiceDetail insertInvoiceDetail)
        {
            return Ok(await _forwardRepository.InsertInvoiceDetails(insertInvoiceDetail));
        }
        #endregion

        #region Sales Return

        [HttpPost]
        public async Task<IActionResult> SalesReturns(SalesReturnProps salesReturnProps)
        {
            return Ok(await this._forwardRepository.SalesReturns(salesReturnProps));
        }

        [HttpGet]
        public async Task<IActionResult> CancelSalesReturn(string CRinvoiceno)
        {
            return Ok(await this._forwardRepository.CancelSalesReturn(CRinvoiceno));
        }

        [HttpGet]
        public async Task<IActionResult> InvoiceCancellation(string Invoiceno)
        {
            return Ok(await this._forwardRepository.InvoiceCancellation(Invoiceno));
        }
        #endregion

        #region Acknowledgment

        [HttpPost]
        public async Task<IActionResult> ConfirmInvoice()
        {
            return Ok(await _forwardRepository.ConfirmInvoice());
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmInvoiceDetails()
        {
            return Ok(await _forwardRepository.ConfirmInvoiceDetails());
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmQty(InvoiceUpdate invoiceUpdate)
        {
            return Ok(await _forwardRepository.ConfirmQty(invoiceUpdate));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQty(InvoiceUpdate invoiceUpdate)
        {
            return Ok(await _forwardRepository.UpdateQty(invoiceUpdate));
        }

        [HttpPost]
        public async Task<IActionResult> ReuploadLR()
        {
            return Ok(await _forwardRepository.ReUploadLR());
        }
        #endregion
    }
}
