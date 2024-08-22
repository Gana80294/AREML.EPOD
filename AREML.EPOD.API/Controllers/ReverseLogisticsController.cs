using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.ReverseLogistics;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReverseLogisticsController : ControllerBase
    {
        private IReverseLogisticsRepository _reverseLogisticsRepository;
        public ReverseLogisticsController(IReverseLogisticsRepository reverseLogisticsRepository)
        {
            _reverseLogisticsRepository = reverseLogisticsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReversePODByCustomer(string customerCode)
        {
            return Ok(await this._reverseLogisticsRepository.GetAllReversePODByCustomer(customerCode));
        }

        [HttpGet]
        public async Task<IActionResult> GetReverseMaterialDetails(int headerId)
        {
            return Ok(await this._reverseLogisticsRepository.GetReverseMaterialDetails(headerId));
        }

        [HttpGet]
        public async Task<IActionResult> GetLRDetailsByHeaderId(int headerId)
        {
            return Ok(await this._reverseLogisticsRepository.GetLRDetailsByHeaderId(headerId));
        }

        [HttpPost]
        public async Task<IActionResult> FilterReversePODDetails(ReversePodFilterClass filterClass)
        {
            return Ok(await this._reverseLogisticsRepository.FilterReversePODDetails(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadRPODDocuments(int attachmentId)
        {
            return Ok(await this._reverseLogisticsRepository.DownloadRPODDocuments(attachmentId));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadRPODReport(ReversePodFilterClass filterClass)
        {
            return Ok(await this._reverseLogisticsRepository.DownloadRPODReport(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReversePodApprover(List<Guid> approvers)
        {
            return Ok(await this._reverseLogisticsRepository.UpdateReversePodApprover(approvers));
        }

        [HttpGet]
        public async Task<IActionResult> GetIsApprover(Guid UserId)
        {
            return Ok(await this._reverseLogisticsRepository.GetIsApprover(UserId));
        }

        [HttpGet]
        public async Task<IActionResult> GetDcApprovers()
        {
            return Ok(await this._reverseLogisticsRepository.GetDcApprovers());
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmReversePodDirectly()
        {
            return Ok(await this._reverseLogisticsRepository.ConfirmReversePodDirectly());
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmReversePod()
        {
            return Ok(await this._reverseLogisticsRepository.ConfirmReversePod());
        }
    }
}
