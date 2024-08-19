using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]")]
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
    }
}
