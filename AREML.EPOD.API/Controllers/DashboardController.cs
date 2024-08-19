using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private IDashboardRepository _dashboardRepository;
        public DashboardController(IDashboardRepository dashboardRepository)
        {
            this._dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceStatusCount(Guid UserID)
        {
            return Ok(await this._dashboardRepository.GetInvoiceStatusCount(UserID));
        }


        [HttpGet]
        public async Task<IActionResult> GetInvoiceStatusCountByUser(string UserCode)
        {
            return Ok(await this._dashboardRepository.GetInvoiceStatusCountByUser(UserCode));
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryCount(Guid UserID)
        {
            return Ok(await this._dashboardRepository.GetDeliveryCount(UserID));
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryCountByUser(string UserCode)
        {
            return Ok(await this._dashboardRepository.GetDeliveryCountByUser(UserCode));
        }

        [HttpGet]
        public async Task<IActionResult> FilterInvoiceStatusCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterInvoiceStatusCount(UserID, Organization, Division, Plant, StartDate, EndDate));
        }


        [HttpGet]
        public async Task<IActionResult> FilterInvoiceStatusCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterInvoiceStatusCountByUser(UserCode, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterLeadTimeCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterLeadTimeCountByUser(UserCode, StartDate, EndDate));
        }


        [HttpGet]
        public async Task<IActionResult> FilterDeliveryCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterDeliveryCount(UserID, Organization, Division, Plant, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterDeliveryCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterDeliveryCountByUser(UserCode, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveredInvoices(Guid UserID, string Condition)
        {
            return Ok(await this._dashboardRepository.GetDeliveredInvoices(UserID, Condition));
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceHeaderDetailByUserID(Guid UserID)
        {
            return Ok(await this._dashboardRepository.GetInvoiceHeaderDetailByUserID(UserID));
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoiceHeaderDetailByUsername(string UserCode)
        {
            return Ok(await this._dashboardRepository.GetInvoiceHeaderDetailByUsername(UserCode));
        }

        #region ChartClick

        [HttpGet]
        public async Task<IActionResult> FilterConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterConfirmedInvoices(UserID, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterPartiallyConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterPartiallyConfirmedInvoices(UserID, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }

        [HttpPost]
        public async Task<IActionResult> FilterPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterPartiallyConfirmedInvoices(filterClass));
        }


        [HttpGet]
        public async Task<IActionResult> FilterPendingInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterPendingInvoices(UserID, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterOnTimeDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterOnTimeDeliveryInvoices(UserID, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }


        [HttpPost]
        public async Task<IActionResult> FilterOnTimeDeliveryInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterOnTimeDeliveryInvoices(filterClass));
        }

        [HttpGet]
        public async Task<IActionResult> FilterLateDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterLateDeliveryInvoices(UserID, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }


        [HttpPost]
        public async Task<IActionResult> FilterLateDeliveryInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterLateDeliveryInvoices(filterClass));
        }

        [HttpGet]
        public async Task<IActionResult> FilterConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterConfirmedInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterPartiallyConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterPartiallyConfirmedInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterPendingInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterPendingInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterSavedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterSavedInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }


        [HttpGet]
        public async Task<IActionResult> FilterOnTimeDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterOnTimeDeliveryInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterLateDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterLateDeliveryInvoicesByUser(UserCode, CurrentPage, Records, StartDate, EndDate));
        }

        [HttpGet]
        public async Task<IActionResult> FilterLeadTimeInvoicesByUser(string UserCode, int CurrentPage, int Records, string LeadTime, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterLeadTimeInvoicesByUser(UserCode, CurrentPage, Records, LeadTime, StartDate, EndDate));
        }


        [HttpPost]
        public async Task<IActionResult> FilterWithinLeadTimeInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterWithinLeadTimeInvoices(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterBeyondLeadTimeInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterBeyondLeadTimeInvoices(filterClass));
        }
        #endregion


        #region ChartDataAmararajaUser

        [HttpPost]
        public async Task<IActionResult> FilterInvoicesStatusCount(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterInvoicesStatusCount(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterLeadTimeCount(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterLeadTimeCount(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterDeliverysCount(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterDeliverysCount(filterClass));
        }
        #endregion



        #region FiltersWithFilterClassAmararajaUser

        [HttpPost]
        public async Task<IActionResult> FilterConfirmedInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterConfirmedInvoices(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterSavedInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterSavedInvoices(filterClass));
        }

        [HttpPost]
        public async Task<IActionResult> FilterPendingInvoices(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilterPendingInvoices(filterClass));
        }
        #endregion


        #region Unused

        //[HttpGet]
        //[Route("FilterLeadTimeCount")]
        //public async Task<IActionResult> FilterLeadTimeCountByUser(Guid UserID)
        //{
        //    return Ok(await this._dashboardRepository.FilterLeadTimeCountByUser(UserID));
        //}


        //[HttpGet]
        //[Route("FilterConfirmedInvoices")]
        //public async Task<IActionResult> FilterConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterConfirmedInvoices(UserID, Organization, Division, Plant, StartDate, EndDate));
        //}

        //[HttpGet]
        //[Route("FilterPartiallyConfirmedInvoices")]
        //public async Task<IActionResult> FilterPartiallyConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterPartiallyConfirmedInvoices(UserID, Organization, Division, Plant, StartDate, EndDate));
        //}


        //[HttpGet]
        //[Route("FilterPendingInvoices")]
        //public async Task<IActionResult> FilterPendingInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterPendingInvoices(UserID, Organization, Division, Plant, StartDate, EndDate));
        //}

        //[HttpGet]
        //[Route("FilterOnTimeDeliveryInvoices")]
        //public async Task<IActionResult> FilterOnTimeDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterOnTimeDeliveryInvoices(UserID, Organization, Division, Plant, StartDate, EndDate));
        //}

        //[HttpGet]
        //[Route("FilterLateDeliveryInvoices")]
        //public async Task<IActionResult> FilterLateDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterLateDeliveryInvoices(UserID, Organization, Division, Plant, StartDate, EndDate));
        //}


        //[HttpGet]
        //[Route("FilterConfirmedInvoicesByUser")]
        //public async Task<IActionResult> FilterConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterConfirmedInvoicesByUser(UserCode, StartDate, EndDate));
        //}

        //[HttpGet]
        //[Route("FilterPartiallyConfirmedInvoicesByUser")]
        //public async Task<IActionResult> FilterPartiallyConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterPartiallyConfirmedInvoicesByUser(UserCode, StartDate, EndDate));
        //}


        //[HttpGet]
        //[Route("FilterPendingInvoicesByUser")]
        //public async Task<IActionResult> FilterPendingInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterPendingInvoicesByUser(UserCode, StartDate, EndDate));
        //}


        //[HttpGet]
        //[Route("FilterOnTimeDeliveryInvoicesByUser")]
        //public async Task<IActionResult> FilterOnTimeDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterOnTimeDeliveryInvoicesByUser(UserCode, StartDate, EndDate));
        //}

        //[HttpGet]
        //[Route("FilterLateDeliveryInvoicesByUser")]
        //public async Task<IActionResult> FilterLateDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        //{
        //    return Ok(await this._dashboardRepository.FilterLateDeliveryInvoicesByUser(UserCode, StartDate, EndDate));
        //}
        #endregion

        #region CodeOptimization
        [HttpGet]
        public async Task<IActionResult> FilterConfirmedInvoicess(Guid? UserID, string? UserCode, int CurrentPage, int Records, string? Organization = null, string? Division = null, string? Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            return Ok(await this._dashboardRepository.FilterConfirmedInvoices(UserID, UserCode, CurrentPage, Records, Organization, Division, Plant, StartDate, EndDate));
        }

        [HttpPost]
        public async Task<IActionResult> Filtered(FilterClass filterClass)
        {
            return Ok(await this._dashboardRepository.FilteringInvoices(filterClass));
        }

        #endregion
    }
}
