using AREML.EPOD.Core.Entities.ForwardLogistics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IDashboardRepository
    {
        Task<InvoiceStatusCount> GetInvoiceStatusCount(Guid UserID);

        Task<InvoiceStatusCount> GetInvoiceStatusCountByUser(string UserCode);
        Task<DeliveryCount> GetDeliveryCount(Guid UserID);

        Task<DeliveryCount> GetDeliveryCountByUser(string UserCode);

        Task<InvoiceStatusCount> FilterInvoiceStatusCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<InvoiceStatusCount> FilterInvoiceStatusCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);
        Task<List<int>> FilterLeadTimeCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<DeliveryCount> FilterDeliveryCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<DeliveryCount> FilterDeliveryCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<P_INV_HEADER_DETAIL>> GetDeliveredInvoices(Guid UserID, string Condition);

        Task<List<InvoiceHeaderDetails>> GetInvoiceHeaderDetailByUserID(Guid UserID);

        Task<List<InvoiceHeaderDetails>> GetInvoiceHeaderDetailByUsername(string UserCode);

        #region ChartClick
        Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);


        Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(FilterClass filterClass);
        Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterPendingInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterSavedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterLeadTimeInvoicesByUser(string UserCode, int CurrentPage, int Records, string LeadTime, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilterWithinLeadTimeInvoices(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterBeyondLeadTimeInvoices(FilterClass filterClass);
        #endregion


        #region ChartDataAmararajaUser
        Task<InvoiceStatusCount> FilterInvoicesStatusCount(FilterClass filterClass);
        Task<List<int>> FilterLeadTimeCount(FilterClass filterClass);
        Task<DeliveryCount> FilterDeliverysCount(FilterClass filterClass);
        #endregion


        #region FiltersWithFilterClassAmararajaUser
        Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(FilterClass filterClass);

        Task<List<InvoiceHeaderDetails>> FilterSavedInvoices(FilterClass filterClass);
        Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(FilterClass filterClass);

        #endregion


        #region Unused
        //Task<List<int>> FilterLeadTimeCountByUser(Guid UserID);

        //Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);
        //Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterPendingInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);

        //Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);
        //Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null);
        #endregion

        #region CodeOptimization
        Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid? UserID, string? UserCode, int CurrentPage, int Records, string? Organization = null, string? Division = null, string? Plant = null, DateTime? StartDate = null, DateTime? EndDate = null);

        Task<List<InvoiceHeaderDetails>> FilteringInvoices(FilterClass filterClass);
        #endregion
    }
}
