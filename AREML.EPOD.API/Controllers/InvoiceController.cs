using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AREML.EPOD.Interfaces.IRepositories;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Dtos.ReverseLogistics;
namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private IInvoiceRepository _invoiceRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet]     
        public async Task<IActionResult> GetAllInvoiceDetails(Guid UserID)
        {
            return Ok(await _invoiceRepository.GetAllInvoiceDetails(UserID));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInvoices(DeleteInvoiceDto payload)
        {
            try
            {
                var result = await _invoiceRepository.DeleteInvoices(payload);
                if (result)
                {
                    return Ok("Invoice deleted successfully.");
                }
                else
                {
                    return BadRequest("Invoice could not be deleted.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceDetailByInvNo(string InvNo)
        {
            return Ok(await _invoiceRepository.GetInvoiceDetailByInvNo(InvNo));
        }
    }
}
