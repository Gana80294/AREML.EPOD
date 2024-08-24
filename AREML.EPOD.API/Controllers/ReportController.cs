using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private IReportRepository _reportRepository;
        public ReportController(IReportRepository reportRepository)
        {
            this._reportRepository = reportRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetDivisions()
        {
            return Ok(await this._reportRepository.GetDivisions());
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingInvoiceDetails(Guid UserID)
        {
            return Ok(await this._reportRepository.GetPendingInvoiceDetails(UserID));
        }


        [HttpGet]
        public async Task<IActionResult> GetFilteredInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            return Ok(await this._reportRepository.GetFilteredInvoiceDetails(UserID, Status, StartDate, EndDate, InvoiceNumber, Organization, Division, Plant, CustomerName));
        }

    

        [HttpPost]
        public async Task<IActionResult> GetFilteredInvoiceDetails(FilterClass filterClass)
        {
            return Ok(await this._reportRepository.GetFilteredInvoiceDetails(filterClass));
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            var fileContent = await this._reportRepository.DownloadInvoiceDetails(UserID, Status, StartDate, EndDate, InvoiceNumber, Organization, Division, Plant, CustomerName);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Invoice_details_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadInvoiceDetails(FilterClass filterClass)
        {
            var fileContent = await this._reportRepository.DownloadInvoiceDetails(filterClass);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Invoice_details_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> DownloadInvoiceDetailsForAutomation(FilterClass filterClass)
        {
            var fileContent = await this._reportRepository.DownloadInvoiceDetailsForAutomation(filterClass);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Invoice_details_automation_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpGet]
        public async Task<IActionResult> DowloadPODDocument(int HeaderID, int AttachmentID)
        {
            return Ok(await this._reportRepository.DowloadPODDocument(HeaderID, AttachmentID));
        }

        #region UnUsed
        [HttpGet]
        public async Task<IActionResult> GetFilteredInvoicedetails(Guid UserID, int CurrentPage, int Records, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null, string CustomerGroup = null)
        {
            return Ok(await this._reportRepository.GetFilteredInvoiceDetails(UserID, CurrentPage, Records, Status, StartDate, EndDate, InvoiceNumber, Organization, Division, Plant, CustomerName, CustomerGroup));
        }
        #endregion
    }
}
