using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MobileController : ControllerBase
    {

        private IMobileRepository _mobileRepository;
        public MobileController(IMobileRepository mobileRepository)
        {
            this._mobileRepository = mobileRepository;
        }

        [HttpPost]
        public async Task<IActionResult> FilterInvoicesStatusCount(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterInvoicesStatusCount(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterDeliverysCount(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterDeliverysCount(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterLeadTimeCount(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterLeadTimeCount(filterClass));
        }


        [HttpPost]
        public async Task<IActionResult> FilterInvoiceStatusCountByUser(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterInvoiceStatusCountByUser(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterDeliveryCountByUser(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterDeliveryCountByUser(filterClass));
        }


        [HttpPost]
        public async Task<IActionResult> FilterLeadTimeCountByUser(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterLeadTimeCountByUser(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterInvoices(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterInvoices(filterClass));
        }


        [HttpPost]
        public async Task<IActionResult> FilterInvoicesByUser(FilterClass filterClass)
        {
            return Ok(await this._mobileRepository.FilterInvoicesByUser(filterClass));
        }

        [HttpGet]
        [Route("DowloandPODDocument")]
        public async Task<IActionResult> DowloandPODDocument(int HeaderID, int AttachmentID)
        {
            return Ok(await this._mobileRepository.DowloandPODDocument(HeaderID, AttachmentID));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMobileVersion(string version)
        {
            return Ok(await this._mobileRepository.UpdateMobileVersion(version));
        }


        [HttpGet]
        public async Task<IActionResult> GetMobileAppVersion()
        {
            return Ok(await this._mobileRepository.GetMobileAppVersion());
        }
    }
}
