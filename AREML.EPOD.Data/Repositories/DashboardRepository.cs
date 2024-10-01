using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using System.Linq;
using AREML.EPOD.Core.Entities.Master;
using System.Numerics;
using AREML.EPOD.Data.Logging;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AREML.EPOD.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private AuthContext _dbContext;

        public DashboardRepository(AuthContext context)
        {
            this._dbContext = context;

        }

        public async Task<InvoiceStatusCount> GetInvoiceStatusCount(Guid UserID)
        {
            try
            {
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == UserID
                                 select tb3).Distinct().ToListAsync();
                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                        where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE
                                        select new
                                        {
                                            tb.HEADER_ID,
                                            tb.INV_NO,
                                            tb.STATUS,
                                        }).ToListAsync();
                    InvoiceStatusCount.TotalInvoices = result.Count;
                    InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                            where tb.STATUS.ToLower() == "confirmed"
                                                            select tb.INV_NO).Count();
                    InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                     where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                     select tb.INV_NO).Count();
                    InvoiceStatusCount.PendingInvoices = (from tb in result
                                                          where tb.STATUS.ToLower() == "open"
                                                          select tb.INV_NO).Count();
                    InvoiceStatusCount.SavedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "saved"
                                                        select tb.INV_NO).Count();
                    return InvoiceStatusCount;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE
                                        select new
                                        {
                                            tb.HEADER_ID,
                                            tb.INV_NO,
                                            tb.STATUS,
                                        }).ToListAsync();
                    InvoiceStatusCount.TotalInvoices = result.Count;
                    InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                            where tb.STATUS.ToLower() == "confirmed"
                                                            select tb.INV_NO).Count();
                    InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                     where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                     select tb.INV_NO).Count();
                    InvoiceStatusCount.PendingInvoices = (from tb in result
                                                          where tb.STATUS.ToLower() == "open"
                                                          select tb.INV_NO).Count();
                    InvoiceStatusCount.SavedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "saved"
                                                        select tb.INV_NO).Count();
                    return InvoiceStatusCount;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> GetInvoiceStatusCountByUser(string UserCode)
        {
            try
            {
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.STATUS,
                                    }).ToListAsync();
                InvoiceStatusCount.TotalInvoices = result.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in result
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                InvoiceStatusCount.SavedInvoices = (from tb in result
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                return InvoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> GetDeliveryCount(Guid UserID)
        {
            try
            {
                DeliveryCount deliveryCount = new DeliveryCount();
                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == UserID
                                 select tb3).Distinct().ToListAsync();
                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                        where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed")
                                        select new
                                        {
                                            tb.HEADER_ID,
                                            tb.INV_NO,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.PROPOSED_DELIVERY_DATE
                                        }).ToListAsync();
                    deliveryCount.TotalDelivery = result.Count;
                    deliveryCount.InLineDelivery = (from tb in result
                                                    where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                    select tb.INV_NO).Count();
                    deliveryCount.DelayedDelivery = (from tb in result
                                                     where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.ACTUAL_DELIVERY_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                     select tb.INV_NO).Count();
                    return deliveryCount;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed")
                                        select new
                                        {
                                            tb.HEADER_ID,
                                            tb.INV_NO,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.PROPOSED_DELIVERY_DATE
                                        }).ToListAsync();
                    deliveryCount.TotalDelivery = result.Count;
                    deliveryCount.InLineDelivery = (from tb in result
                                                    where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                    select tb.INV_NO).Count();
                    deliveryCount.DelayedDelivery = (from tb in result
                                                     where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.ACTUAL_DELIVERY_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                     select tb.INV_NO).Count();
                    return deliveryCount;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> GetDeliveryCountByUser(string UserCode)
        {
            try
            {
                DeliveryCount deliveryCount = new DeliveryCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed") && tb.IS_ACTIVE
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.ACTUAL_DELIVERY_DATE,
                                        tb.PROPOSED_DELIVERY_DATE
                                    }).ToListAsync();
                deliveryCount.TotalDelivery = result.Count;
                deliveryCount.InLineDelivery = (from tb in result
                                                where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.INV_NO).Count();
                deliveryCount.DelayedDelivery = (from tb in result
                                                 where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.ACTUAL_DELIVERY_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.INV_NO).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> FilterInvoiceStatusCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.STATUS,
                                    }).ToListAsync();
                InvoiceStatusCount.TotalInvoices = result.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in result
                                                      where !tb.STATUS.ToLower().Contains("confirmed")
                                                      select tb.INV_NO).Count();
                return InvoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> FilterInvoiceStatusCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.STATUS,
                                    }).ToListAsync();
                InvoiceStatusCount.TotalInvoices = result.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.SavedInvoices = (from tb in result
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in result
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                return InvoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<int>> FilterLeadTimeCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                List<int> Result = new List<int>();
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    tb.STATUS.ToLower() == "open"
                                    select new
                                    {
                                        tb.PROPOSED_DELIVERY_DATE
                                    }).ToListAsync();
                Result.Add(result.Count);
                Result.Add(result.Where(t => t.PROPOSED_DELIVERY_DATE == null || DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE).Count());//within
                Result.Add(result.Where(t => t.PROPOSED_DELIVERY_DATE != null && DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE).Count());//beyond

                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<DeliveryCount> FilterDeliveryCount(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                DeliveryCount deliveryCount = new DeliveryCount();

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    tb.ACTUAL_DELIVERY_DATE.HasValue &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                                    (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower().Contains("confirmed")
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.ACTUAL_DELIVERY_DATE,
                                        tb.PROPOSED_DELIVERY_DATE,
                                        tb.VEHICLE_REPORTED_DATE
                                    }).ToListAsync();
                deliveryCount.TotalDelivery = result.Count;
                deliveryCount.InLineDelivery = (from tb in result
                                                where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.INV_NO).Count();
                deliveryCount.DelayedDelivery = (from tb in result
                                                 where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.INV_NO).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> FilterDeliveryCountByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                DeliveryCount deliveryCount = new DeliveryCount();
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    tb.STATUS.ToLower() != "open"
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.INV_NO,
                                        tb.ACTUAL_DELIVERY_DATE,
                                        tb.PROPOSED_DELIVERY_DATE,
                                        tb.VEHICLE_REPORTED_DATE
                                    }).ToListAsync();
                deliveryCount.TotalDelivery = result.Count;
                deliveryCount.InLineDelivery = (from tb in result
                                                where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.HEADER_ID).Count();
                deliveryCount.DelayedDelivery = (from tb in result
                                                 where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.HEADER_ID).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetDeliveredInvoices(Guid UserID, string Condition)
        {
            try
            {
                var condition = Condition?.ToLower();
                var userResult = (from td in _dbContext.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                switch (condition)
                {
                    case "inlinedelivery":
                        var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                            where (isAllOrganization || Organizations.Any(y => y == tb.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == tb.PLANT)) && tb.ACTUAL_DELIVERY_DATE.HasValue && tb.IS_ACTIVE &&
                                            (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date) &&
                                            tb.STATUS.ToLower().Contains("confirmed")
                                            select tb).ToListAsync();
                        return result;
                    case "delayeddelivery":
                        var result1 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                             where (isAllOrganization || Organizations.Any(y => y == tb.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == tb.PLANT)) && tb.ACTUAL_DELIVERY_DATE.HasValue && tb.IS_ACTIVE &&
                                             (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.ACTUAL_DELIVERY_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date) &&
                                             tb.STATUS.ToLower().Contains("confirmed")
                                             select tb).ToListAsync();
                        return result1;
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> GetInvoiceHeaderDetailByUserID(Guid UserID)
        {
            try
            {
                var userResult = (from td in _dbContext.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where (isAllOrganization || Organizations.Any(y => y == tb.ORGANIZATION)) && (tb.IS_ACTIVE) && (isAllPlant || Plants.Any(y => y == tb.PLANT))
                              orderby tb.HEADER_ID descending
                              select new InvoiceHeaderDetails
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  PLANT = tb.PLANT,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS
                              }).Take(5).ToList();
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> GetInvoiceHeaderDetailByUsername(string UserCode)
        {
            try
            {
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE
                                    orderby tb.HEADER_ID descending
                                    select new InvoiceHeaderDetails
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        PLANT = tb.PLANT,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS
                                    }).Take(5).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region ChartClick

        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                if (!StartDate.HasValue)
                {
                    StartDate = DateTime.Now.AddDays(-30);
                }
                if (!EndDate.HasValue)
                {
                    EndDate = DateTime.Now;
                }

                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    tb.STATUS.ToLower() == "confirmed" &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    tb.STATUS.ToLower() == "partiallyconfirmed" &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }
                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "partiallyconfirmed" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            tb.DRIVER_CONTACT,
                                            tb.TRACKING_LINK,
                                            tb.TOTAL_TRAVEL_TIME,
                                            tb.TOTAL_DISTANCE,
                                            tb.DELIVERY_DATE,
                                            tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var data = (from tb in result
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                    TRACKING_LINK = tb.TRACKING_LINK,
                                    TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                    TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                    DELIVERY_DATE = tb.DELIVERY_DATE,
                                    DELIVERY_TIME = tb.DELIVERY_TIME,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "partiallyconfirmed" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var headerIds = result.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    !tb.STATUS.ToLower().Contains("confirmed") &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)

                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    tb.ACTUAL_DELIVERY_DATE.HasValue &&
                                     (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                                    (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower() != "open"
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var result1 = (from tb in result
                               where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               orderby tb.HEADER_ID
                               select tb).Skip(SkipValue).Take(TakeValue).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;


                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID

                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() != "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            tb.DRIVER_CONTACT,
                                            tb.TRACKING_LINK,
                                            tb.TOTAL_TRAVEL_TIME,
                                            tb.TOTAL_DISTANCE,
                                            tb.DELIVERY_DATE,
                                            tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = (from tb in result
                                   where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                   select tb).Skip(SkipValue).Take(TakeValue).ToList();
                    var data = (from tb in result1
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                    TRACKING_LINK = tb.TRACKING_LINK,
                                    TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                    TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                    DELIVERY_DATE = tb.DELIVERY_DATE,
                                    DELIVERY_TIME = tb.DELIVERY_TIME,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() != "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = (from tb in result
                                   where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                   select tb).Skip(SkipValue).Take(TakeValue).ToList();
                    var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result1)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(Guid UserID, int CurrentPage, int Records, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                    tb.ACTUAL_DELIVERY_DATE.HasValue &&
                                     (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                                    (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower() != "open"
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.PROPOSED_DELIVERY_DATE.Value.Date > tb.VEHICLE_REPORTED_DATE.Value.Date)
                               orderby tb.HEADER_ID
                               select tb).Skip(SkipValue).Take(TakeValue).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID

                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() != "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            tb.DRIVER_CONTACT,
                                            tb.TRACKING_LINK,
                                            tb.TOTAL_TRAVEL_TIME,
                                            tb.TOTAL_DISTANCE,
                                            tb.DELIVERY_DATE,
                                            tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();
                    var result1 = (from tb in result
                                   where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                   orderby tb.HEADER_ID
                                   select tb).Skip(SkipValue).Take(TakeValue).ToList();
                    var data = (from tb in result1
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                    TRACKING_LINK = tb.TRACKING_LINK,
                                    TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                    TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                    DELIVERY_DATE = tb.DELIVERY_DATE,
                                    DELIVERY_TIME = tb.DELIVERY_TIME,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() != "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();
                    var result1 = (from tb in result
                                   where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                   orderby tb.HEADER_ID
                                   select tb).Skip(SkipValue).Take(TakeValue).ToList();
                    var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result1)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result1;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "confirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                var headerIds = result.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                var headerIds = result.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPendingInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "open" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                var headerIds = result.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterSavedInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "saved" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                var headerIds = result.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    tb.STATUS.ToLower() != "open"
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               orderby tb.HEADER_ID
                               select tb).Skip(SkipValue).Take(TakeValue).ToList();
                var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result1)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoicesByUser(string UserCode, int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                     (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    tb.STATUS.ToLower() != "open"
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               orderby tb.HEADER_ID
                               select tb).Skip(SkipValue).Take(TakeValue).ToList();
                var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result1)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterLeadTimeInvoicesByUser(string UserCode, int CurrentPage, int Records, string LeadTime, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                                     (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                    tb.STATUS.ToLower() == "open"
                                    orderby tb.HEADER_ID
                                    select new InvoiceHeaderDetails()
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var result1 = new List<InvoiceHeaderDetails>();
                if (LeadTime == "within")
                {
                    result1 = result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Skip(SkipValue).Take(TakeValue).ToList();
                }
                else if (LeadTime == "beyond")
                {
                    result1 = result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Skip(SkipValue).Take(TakeValue).ToList();
                }
                var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                var attachments = await _dbContext.P_INV_ATTACHMENT
                    .Where(att => headerIds.Contains(att.HEADER_ID))
                    .Select(att => new
                    {
                        att.HEADER_ID,
                        att.ATTACHMENT_ID,
                        att.FILE_NAME
                    }).ToListAsync();

                var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                foreach (var header in result1)
                {
                    if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                    {
                        header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        header.ATTACHMENT_NAME = att.FILE_NAME;
                    }
                }
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterWithinLeadTimeInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;


                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID

                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            tb.DRIVER_CONTACT,
                                            tb.TRACKING_LINK,
                                            tb.TOTAL_TRAVEL_TIME,
                                            tb.TOTAL_DISTANCE,
                                            tb.DELIVERY_DATE,
                                            tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Skip(SkipValue).Take(TakeValue).ToList();
                    var data = (from tb in result1
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                    TRACKING_LINK = tb.TRACKING_LINK,
                                    TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                    TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                    DELIVERY_DATE = tb.DELIVERY_DATE,
                                    DELIVERY_TIME = tb.DELIVERY_TIME,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Skip(SkipValue).Take(TakeValue).ToList();
                    var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result1)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterBeyondLeadTimeInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;

                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID

                                 select tb3).Distinct().ToListAsync();

                var plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            tb.DRIVER_CONTACT,
                                            tb.TRACKING_LINK,
                                            tb.TOTAL_TRAVEL_TIME,
                                            tb.TOTAL_DISTANCE,
                                            tb.DELIVERY_DATE,
                                            tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Skip(SkipValue).Take(TakeValue).ToList();
                    var data = (from tb in result1
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                    TRACKING_LINK = tb.TRACKING_LINK,
                                    TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                    TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                    DELIVERY_DATE = tb.DELIVERY_DATE,
                                    DELIVERY_TIME = tb.DELIVERY_TIME,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).ToListAsync();

                    var result1 = result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Skip(SkipValue).Take(TakeValue).ToList();
                    var headerIds = result1.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result1)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region ChartDataAmararajaUser

        public async Task<InvoiceStatusCount> FilterInvoicesStatusCount(FilterClass filterClass)
        {
            try
            {
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;

                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                LogWriter.WriteToFile("Query starts1");
                LogWriter.WriteToFile($"cgs {JsonConvert.SerializeObject(cgs)}");
                var query = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                            join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                            join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                            join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                            where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID
                            select new
                            {
                                tb.HEADER_ID,
                                tb.INV_NO,
                                tb.INV_DATE,
                                tb.STATUS,
                                tb.ORGANIZATION,
                                tb.DIVISION,
                                tb.PLANT,
                                tb.CUSTOMER_GROUP_DESC,
                                tb.CUSTOMER_NAME
                            }).ToListAsync();
                if (cgs.Count() > 0)
                {
                    LogWriter.WriteToFile("Query with cgs join starts");
                    LogWriter.WriteToFile($"cgs {JsonConvert.SerializeObject(cgs)}");

                    query = (from tb in query
                            join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                            select tb).ToList();
                }
                //LogWriter.WriteToFile("Query before :- " + query.ToQueryString());
                if (isFromDate && filterClass.StartDate.HasValue)
                {
                    query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date).ToList();
                }

                if (isEndDate && filterClass.EndDate.HasValue)
                {
                    query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date).ToList();
                }

                if (isOrganization && filterClass.Organization != null && filterClass.Organization.Any())
                {
                    query = query.Where(tb => filterClass.Organization.Contains(tb.ORGANIZATION)).ToList();
                }

                if (isDivision && filterClass.Division != null && filterClass.Division.Any())
                {
                    query = query.Where(tb => filterClass.Division.Contains(tb.DIVISION)).ToList();
                }

                if (isPlant && filterClass.PlantList != null && filterClass.PlantList.Any())
                {
                    query = query.Where(tb => filterClass.PlantList.Contains(tb.PLANT)).ToList();
                }

                if (isPlantGroup && plants != null && plants.Any())
                {
                    query = query.Where(tb => plants.Contains(tb.PLANT)).ToList();
                }

                if (isCustomerGroup && filterClass.CustomerGroup != null && filterClass.CustomerGroup.Any())
                {
                    query = query.Where(tb => filterClass.CustomerGroup.Contains(tb.CUSTOMER_GROUP_DESC)).ToList();
                }

                if (isCustomerName && !string.IsNullOrEmpty(filterClass.CustomerName))
                {
                    query = query.Where(tb => tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower())).ToList();
                }
                //LogWriter.WriteToFile("Query After :- " + query.ToQueryString());
                var result = query;

                InvoiceStatusCount.TotalInvoices = result.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.SavedInvoices = (from tb in result
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in result
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                return InvoiceStatusCount;

                //if (cgs.Count() > 0)
                //{
                //    LogWriter.WriteToFile("Query starts1");
                //    LogWriter.WriteToFile($"cgs {JsonConvert.SerializeObject(cgs)}");
                //    var query = from tb in _dbContext.P_INV_HEADER_DETAIL
                //                join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                //                join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                //                join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                //                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                //                where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID
                //                select new
                //                {
                //                    tb.HEADER_ID,
                //                    tb.INV_NO,
                //                    tb.INV_DATE,
                //                    tb.STATUS,
                //                    tb.ORGANIZATION,
                //                    tb.DIVISION,
                //                    tb.PLANT,
                //                    tb.CUSTOMER_GROUP_DESC,
                //                    tb.CUSTOMER_NAME
                //                };
                //    LogWriter.WriteToFile("Query before :- " + query.ToQueryString());
                //    if (isFromDate && filterClass.StartDate.HasValue)
                //    {
                //        query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date);
                //    }

                //    if (isEndDate && filterClass.EndDate.HasValue)
                //    {
                //        query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date);
                //    }

                //    if (isOrganization && filterClass.Organization != null && filterClass.Organization.Any())
                //    {
                //        query = query.Where(tb => filterClass.Organization.Contains(tb.ORGANIZATION));
                //    }

                //    if (isDivision && filterClass.Division != null && filterClass.Division.Any())
                //    {
                //        query = query.Where(tb => filterClass.Division.Contains(tb.DIVISION));
                //    }

                //    if (isPlant && filterClass.PlantList != null && filterClass.PlantList.Any())
                //    {
                //        query = query.Where(tb => filterClass.PlantList.Contains(tb.PLANT));
                //    }

                //    if (isPlantGroup && plants != null && plants.Any())
                //    {
                //        query = query.Where(tb => plants.Contains(tb.PLANT));
                //    }

                //    if (isCustomerGroup && filterClass.CustomerGroup != null && filterClass.CustomerGroup.Any())
                //    {
                //        query = query.Where(tb => filterClass.CustomerGroup.Contains(tb.CUSTOMER_GROUP_DESC));
                //    }

                //    if (isCustomerName && !string.IsNullOrEmpty(filterClass.CustomerName))
                //    {
                //        query = query.Where(tb => tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()));
                //    }
                //    LogWriter.WriteToFile("Query After :- " + query.ToQueryString());
                //    var result = await query.ToListAsync();

                //    InvoiceStatusCount.TotalInvoices = result.Count;
                //    InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                //                                            where tb.STATUS.ToLower() == "confirmed"
                //                                            select tb.INV_NO).Count();
                //    InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                //                                                     where tb.STATUS.ToLower() == "partiallyconfirmed"
                //                                                     select tb.INV_NO).Count();
                //    InvoiceStatusCount.SavedInvoices = (from tb in result
                //                                        where tb.STATUS.ToLower() == "saved"
                //                                        select tb.INV_NO).Count();
                //    InvoiceStatusCount.PendingInvoices = (from tb in result
                //                                          where tb.STATUS.ToLower() == "open"
                //                                          select tb.INV_NO).Count();
                //    return InvoiceStatusCount;
                //}
                //else
                //{
                //    LogWriter.WriteToFile("Query starts");
                //    var query = from tb in _dbContext.P_INV_HEADER_DETAIL
                //                join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                //                join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                //                join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                //                where tb3.UserID == filterClass.UserID
                //                && tb2.UserID == tb3.UserID
                //                && tb.IS_ACTIVE
                //                select new
                //                {
                //                    tb.HEADER_ID,
                //                    tb.INV_NO,
                //                    tb.INV_DATE,
                //                    tb.STATUS,
                //                    tb.ORGANIZATION,
                //                    tb.DIVISION,
                //                    tb.PLANT,
                //                    tb.CUSTOMER_GROUP_DESC,
                //                    tb.CUSTOMER_NAME
                //                };
                //    LogWriter.WriteToFile("Query before :- " + query.ToQueryString());

                //    // Apply dynamic filters
                //    if (isFromDate && filterClass.StartDate.HasValue)
                //    {
                //        query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date);
                //    }

                //    if (isEndDate && filterClass.EndDate.HasValue)
                //    {
                //        query = query.Where(tb => tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date);
                //    }

                //    if (isOrganization && filterClass.Organization != null && filterClass.Organization.Any())
                //    {
                //        query = query.Where(tb => filterClass.Organization.Contains(tb.ORGANIZATION));
                //    }

                //    if (isDivision && filterClass.Division != null && filterClass.Division.Any())
                //    {
                //        query = query.Where(tb => filterClass.Division.Contains(tb.DIVISION));
                //    }

                //    if (isPlant && filterClass.PlantList != null && filterClass.PlantList.Any())
                //    {
                //        query = query.Where(tb => filterClass.PlantList.Contains(tb.PLANT));
                //    }

                //    if (isPlantGroup && plants != null && plants.Any())
                //    {
                //        query = query.Where(tb => plants.Contains(tb.PLANT));
                //    }

                //    if (isCustomerGroup && filterClass.CustomerGroup != null && filterClass.CustomerGroup.Any())
                //    {
                //        query = query.Where(tb => filterClass.CustomerGroup.Contains(tb.CUSTOMER_GROUP_DESC));
                //    }

                //    if (isCustomerName && !string.IsNullOrEmpty(filterClass.CustomerName))
                //    {
                //        query = query.Where(tb => tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()));
                //    }
                //    LogWriter.WriteToFile("Query after :- " + query.ToQueryString());
                //    // Execute the query
                //    var result = await query.ToListAsync();

                //    InvoiceStatusCount.TotalInvoices = result.Count;
                //    InvoiceStatusCount.ConfirmedInvoices = (from tb in result
                //                                            where tb.STATUS.ToLower() == "confirmed"
                //                                            select tb.INV_NO).Count();
                //    InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in result
                //                                                     where tb.STATUS.ToLower() == "partiallyconfirmed"
                //                                                     select tb.INV_NO).Count();
                //    InvoiceStatusCount.SavedInvoices = (from tb in result
                //                                        where tb.STATUS.ToLower() == "saved"
                //                                        select tb.INV_NO).Count();
                //    InvoiceStatusCount.PendingInvoices = (from tb in result
                //                                          where tb.STATUS.ToLower() == "open"
                //                                          select tb.INV_NO).Count();
                //    return InvoiceStatusCount;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<int>> FilterLeadTimeCount(FilterClass filterClass)
        {
            try
            {
                List<int> Result = new List<int>();
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                DeliveryCount deliveryCount = new DeliveryCount();
                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID && tb.STATUS.ToLower() == "open" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC)) &&
                                         (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        select new
                                        {
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.CUSTOMER_GROUP_DESC
                                        }).ToListAsync();
                    result = (from tb in result
                              join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                              select tb).ToList();
                    Result.Add(result.Count);
                    Result.Add(result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Count());
                    Result.Add(result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Count());
                    return Result;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && tb.STATUS.ToLower() == "open" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        select new
                                        {
                                            tb.PROPOSED_DELIVERY_DATE
                                        }).ToListAsync();
                    Result.Add(result.Count);
                    Result.Add(result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Count());
                    Result.Add(result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Count());
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> FilterDeliverysCount(FilterClass filterClass)
        {
            try
            {
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                DeliveryCount deliveryCount = new DeliveryCount();
                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                    join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                    join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                    where tb3.UserID == filterClass.UserID &&
                                    tb2.UserID == tb3.UserID &&
                                    tb.IS_ACTIVE && tb.STATUS.ToLower() != "open" &&

                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                    (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                    && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                     && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                    && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                    select new
                                    {
                                        tb.HEADER_ID,
                                        tb.PROPOSED_DELIVERY_DATE,
                                        tb.VEHICLE_REPORTED_DATE,
                                        tb.STATUS,
                                        tb.CUSTOMER_GROUP_DESC
                                    }).ToListAsync();
                if(cgs.Count() > 0)
                {
                    result = (from tb in result
                             join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                             select tb).ToList();
                }
                deliveryCount.TotalDelivery = result.Count;
                deliveryCount.InLineDelivery = (from tb in result
                                                where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.HEADER_ID).Count();
                deliveryCount.DelayedDelivery = (from tb in result
                                                 where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.HEADER_ID).Count();
                return deliveryCount;
                //if (cgs.Count() > 0)
                //{
                //    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                //                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                //                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                //                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                //                        join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                //                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && tb.STATUS.ToLower() != "open" &&

                //                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                //                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                //                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                //                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                //                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                //                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                //                        select new
                //                        {
                //                            tb.HEADER_ID,
                //                            tb.PROPOSED_DELIVERY_DATE,
                //                            tb.VEHICLE_REPORTED_DATE,
                //                            tb.STATUS,
                //                        }).ToListAsync();
                //    deliveryCount.TotalDelivery = result.Count;
                //    deliveryCount.InLineDelivery = (from tb in result
                //                                    where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                //                                    select tb.HEADER_ID).Count();
                //    deliveryCount.DelayedDelivery = (from tb in result
                //                                     where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                //                                     select tb.HEADER_ID).Count();
                //    return deliveryCount;
                //}
                //else
                //{
                //    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                //                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                //                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                //                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                //                        where tb3.UserID == filterClass.UserID &&
                //                        tb2.UserID == tb3.UserID &&
                //                        tb.IS_ACTIVE && tb.STATUS.ToLower() != "open" &&

                //                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                //                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                //                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                //                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                //                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                //                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                //                        select new
                //                        {
                //                            tb.HEADER_ID,
                //                            tb.PROPOSED_DELIVERY_DATE,
                //                            tb.VEHICLE_REPORTED_DATE,
                //                            tb.STATUS,
                //                        }).ToListAsync();
                //    deliveryCount.TotalDelivery = result.Count;
                //    deliveryCount.InLineDelivery = (from tb in result
                //                                    where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                //                                    select tb.HEADER_ID).Count();
                //    deliveryCount.DelayedDelivery = (from tb in result
                //                                     where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                //                                     select tb.HEADER_ID).Count();
                //    return deliveryCount;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion


        #region FiltersWithFilterClassAmararajaUser

        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID &&
                                        tb.STATUS.ToLower() == "confirmed" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var data = (from tb in result
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "confirmed" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var headerIds = result.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterSavedInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();
                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }
                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "saved" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var data = (from tb in result
                                join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                select new InvoiceHeaderDetails()
                                {
                                    HEADER_ID = tb.HEADER_ID,
                                    ORGANIZATION = tb.ORGANIZATION,
                                    DIVISION = tb.DIVISION,
                                    PLANT = tb.PLANT,
                                    PLANT_NAME = tb.PLANT_NAME,
                                    INV_NO = tb.INV_NO,
                                    ODIN = tb.ODIN,
                                    INV_DATE = tb.INV_DATE,
                                    INV_TYPE = tb.INV_TYPE,
                                    CUSTOMER = tb.CUSTOMER,
                                    CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                    VEHICLE_NO = tb.VEHICLE_NO,
                                    VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                    LR_NO = tb.LR_NO,
                                    LR_DATE = tb.LR_DATE,
                                    PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                    VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                    ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                    STATUS = tb.STATUS,
                                    INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                                }).ToList();
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "saved" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var headerIds = result.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }


                if (cgs.Count() > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new 
                                        {
                                            tb.CUSTOMER_GROUP_DESC,
                                            tb.HEADER_ID,
                                            tb.ORGANIZATION,
                                            tb.DIVISION,
                                            tb.PLANT,
                                            tb.PLANT_NAME,
                                            tb.INV_NO,
                                            tb.ODIN,
                                            tb.INV_DATE,
                                            tb.INV_TYPE,
                                            tb.CUSTOMER,
                                            tb.CUSTOMER_NAME,
                                            tb.VEHICLE_NO,
                                            tb.VEHICLE_CAPACITY,
                                            tb.LR_NO,
                                            tb.LR_DATE,
                                            tb.PROPOSED_DELIVERY_DATE,
                                            tb.VEHICLE_REPORTED_DATE,
                                            tb.ACTUAL_DELIVERY_DATE,
                                            tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    LogWriter.WriteToFile($"------------ Pending res count : {result.Count()}----------");
                    var data = (from tb in result
                              join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  INVOICE_QUANTITY = tb.INVOICE_QUANTITY
                              }).ToList();
                    LogWriter.WriteToFile($"------------ Pending data count : {data.Count()}----------");
                    var headerIds = data.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in data)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return data;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID

                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "open" &&

                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                        (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                        orderby tb.HEADER_ID
                                        select new InvoiceHeaderDetails()
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            ORGANIZATION = tb.ORGANIZATION,
                                            DIVISION = tb.DIVISION,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            STATUS = tb.STATUS,
                                            INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                        }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                    var headerIds = result.Select(h => h.HEADER_ID).ToList();
                    var attachments = await _dbContext.P_INV_ATTACHMENT
                        .Where(att => headerIds.Contains(att.HEADER_ID))
                        .Select(att => new
                        {
                            att.HEADER_ID,
                            att.ATTACHMENT_ID,
                            att.FILE_NAME
                        }).ToListAsync();

                    var attachmentDict = attachments.ToDictionary(att => att.HEADER_ID);
                    foreach (var header in result)
                    {
                        if (attachmentDict.TryGetValue(header.HEADER_ID, out var att))
                        {
                            header.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            header.ATTACHMENT_NAME = att.FILE_NAME;
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion



        #region Unused
        public async Task<List<int>> FilterLeadTimeCountByUser(Guid UserID)
        {
            try
            {
                List<int> Result = new List<int>();
                DeliveryCount deliveryCount = new DeliveryCount();
                var cgs = (from tb in _dbContext.Users
                           join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                           join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                           join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                           where tb.UserID == UserID
                           select tb3).Distinct();
                if (cgs.Count() > 0)
                {
                    var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                                  join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                  join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                  join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                  join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                  where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                  tb.STATUS.ToLower() == "open"
                                  select new
                                  {
                                      tb.PROPOSED_DELIVERY_DATE
                                  }).ToList();
                    Result.Add(result.Count);
                    Result.Add(result.Where(t => DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Count());
                    Result.Add(result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Count());
                    return Result;
                }
                else
                {
                    var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                                  join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                  join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                  join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                  where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                  tb.STATUS.ToLower() == "open"
                                  select new
                                  {
                                      tb.PROPOSED_DELIVERY_DATE
                                  }).ToList();
                    Result.Add(result.Count);
                    Result.Add(result.Where(t => DateTime.Now.Date < t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null).Count());
                    Result.Add(result.Where(t => DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null).Count());
                    return Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                              join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                              join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                              where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                              tb.STATUS.ToLower() == "confirmed" &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                              join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                              join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                              where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                              tb.STATUS.ToLower() == "partiallyconfirmed" &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPendingInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                              join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                              join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                              where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                              !tb.STATUS.ToLower().Contains("confirmed") &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                              join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                              join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                              where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                              tb.ACTUAL_DELIVERY_DATE.HasValue &&
                               (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                              (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower() != "open"
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                var result1 = (from tb in result
                               where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               select tb).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoices(Guid UserID, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                              join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                              join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                              where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                              tb.ACTUAL_DELIVERY_DATE.HasValue &&
                               (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                              (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower() != "open"
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               select tb).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                StartDate = DateTime.Now.AddDays(-30);
                EndDate = DateTime.Now;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "confirmed" && tb.IS_ACTIVE &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterPartiallyConfirmedInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.CUSTOMER == UserCode && tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterPendingInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.CUSTOMER == UserCode && !tb.STATUS.ToLower().Contains("confirmed") && tb.IS_ACTIVE &&
                              (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterOnTimeDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                               (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              tb.STATUS.ToLower() != "open"
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               select tb).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterLateDeliveryInvoicesByUser(string UserCode, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;

                var result = (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                               (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                              (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                              tb.STATUS.ToLower() != "open"
                              select new InvoiceHeaderDetails()
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  DIVISION = tb.DIVISION,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  STATUS = tb.STATUS,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME,
                                  INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                              }).ToList();
                var result1 = (from tb in result
                               where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                               select tb).ToList();
                return result1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        #region CodeOptimization
        public async Task<List<InvoiceHeaderDetails>> FilterConfirmedInvoices(Guid? UserID, string? UserCode, int CurrentPage, int Records, string? Organization = null, string? Division = null, string? Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                var query = _dbContext.P_INV_HEADER_DETAIL.AsQueryable();

                if (UserID.HasValue)
                {
                    if (!StartDate.HasValue)
                    {
                        StartDate = DateTime.Now.AddDays(-30);
                    }
                    if (!EndDate.HasValue)
                    {
                        EndDate = DateTime.Now;
                    }
                    query = from tb in query
                            join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                            join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                            join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                            where tb3.UserID == UserID.Value &&
                                  tb2.UserID == tb3.UserID &&
                                  tb.IS_ACTIVE &&
                                  tb.STATUS.ToLower() == "confirmed" &&
                                  (!StartDate.HasValue || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                  (!EndDate.HasValue || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                                  (string.IsNullOrEmpty(Organization) || tb.ORGANIZATION == Organization) &&
                                  (string.IsNullOrEmpty(Division) || Division.ToLower() == "all" || tb.DIVISION == Division) &&
                                  (string.IsNullOrEmpty(Plant) || tb.PLANT == Plant)
                            select tb;
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    query = from tb in query
                            where tb.CUSTOMER == UserCode &&
                                  tb.STATUS.ToLower() == "confirmed" &&
                                  tb.IS_ACTIVE &&
                                  (!StartDate.HasValue || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                  (!EndDate.HasValue || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                            select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode must be provided.");
                }

                var result = await query
                    .OrderBy(tb => tb.HEADER_ID)
                    .Select(tb => new InvoiceHeaderDetails
                    {
                        HEADER_ID = tb.HEADER_ID,
                        ORGANIZATION = tb.ORGANIZATION,
                        DIVISION = tb.DIVISION,
                        PLANT = tb.PLANT,
                        PLANT_NAME = tb.PLANT_NAME,
                        INV_NO = tb.INV_NO,
                        ODIN = tb.ODIN,
                        INV_DATE = tb.INV_DATE,
                        INV_TYPE = tb.INV_TYPE,
                        CUSTOMER = tb.CUSTOMER,
                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                        VEHICLE_NO = tb.VEHICLE_NO,
                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                        LR_NO = tb.LR_NO,
                        LR_DATE = tb.LR_DATE,
                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                        STATUS = tb.STATUS,
                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                        TRACKING_LINK = tb.TRACKING_LINK,
                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                        DELIVERY_DATE = tb.DELIVERY_DATE,
                        DELIVERY_TIME = tb.DELIVERY_TIME,
                        INVOICE_QUANTITY = _dbContext.P_INV_ITEM_DETAIL
                            .Where(items => items.HEADER_ID == tb.HEADER_ID)
                            .Sum(items => items.QUANTITY)
                    })
                    .Skip(SkipValue)
                    .Take(TakeValue)
                    .ToListAsync();

                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilteringInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlantList = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isPlant = filterClass.Plant != null && filterClass.Plant.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

                if (!filterClass.StartDate.HasValue)
                {
                    filterClass.StartDate = DateTime.Now.AddDays(-30);
                }
                if (!filterClass.EndDate.HasValue)
                {
                    filterClass.EndDate = DateTime.Now;
                }
                IQueryable<P_INV_HEADER_DETAIL> result = null;

                var cgs = await (from tb in _dbContext.Users
                                 join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                 join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                 join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                 where tb.UserID == filterClass.UserID
                                 select tb3).Distinct().ToListAsync();

                List<string> plantCodes = new List<string>();
                if (filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0)
                {
                    plantCodes = await (from tb in _dbContext.PlantGroupPlantMaps
                                        join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                        join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                        where filterClass.PlantGroupList.Contains(tb1.name)
                                        select tb.PlantCode).Distinct().ToListAsync();
                }

                if (cgs.Count() > 0)
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                             join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                             join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                             join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                             where tb3.UserID == filterClass.UserID || tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && tb2.UserID == tb3.UserID &&
                             filterClass.Status.Any(x => x == tb.STATUS.ToLower()) &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                             (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION))
                             && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                             && (!isPlantList || filterClass.PlantList.Any(x => x == tb.PLANT))
                             && (!isPlant || filterClass.Plant.Any(x => x == tb.PLANT))
                             && (!isPlantGroup || plantCodes.Any(x => x == tb.PLANT))
                             && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                             && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                             select tb;
                }
                else
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                             join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                             join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                             where tb3.UserID == filterClass.UserID || tb.CUSTOMER == filterClass.UserCode && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                             filterClass.Status.Any(x => x == tb.STATUS.ToLower()) &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                             (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION))
                             && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                              && (!isPlantList || filterClass.PlantList.Any(x => x == tb.PLANT))
                             && (!isPlant || filterClass.Plant.Any(x => x == tb.PLANT))
                              && (!isPlantGroup || plantCodes.Any(x => x == tb.PLANT))
                             && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                             && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                             select tb;
                }

                var query = await result
                   .OrderBy(tb => tb.HEADER_ID)
                   .Select(tb => new InvoiceHeaderDetails
                   {
                       HEADER_ID = tb.HEADER_ID,
                       ORGANIZATION = tb.ORGANIZATION,
                       DIVISION = tb.DIVISION,
                       PLANT = tb.PLANT,
                       PLANT_NAME = tb.PLANT_NAME,
                       INV_NO = tb.INV_NO,
                       ODIN = tb.ODIN,
                       INV_DATE = tb.INV_DATE,
                       INV_TYPE = tb.INV_TYPE,
                       CUSTOMER = tb.CUSTOMER,
                       CUSTOMER_NAME = tb.CUSTOMER_NAME,
                       VEHICLE_NO = tb.VEHICLE_NO,
                       VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                       LR_NO = tb.LR_NO,
                       LR_DATE = tb.LR_DATE,
                       PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                       VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                       ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                       STATUS = tb.STATUS,
                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                       TRACKING_LINK = tb.TRACKING_LINK,
                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                       DELIVERY_DATE = tb.DELIVERY_DATE,
                       DELIVERY_TIME = tb.DELIVERY_TIME,
                       INVOICE_QUANTITY = _dbContext.P_INV_ITEM_DETAIL
                           .Where(items => items.HEADER_ID == tb.HEADER_ID)
                           .Sum(items => items.QUANTITY)
                   })
                   .Skip(SkipValue)
                   .Take(TakeValue)
                   .ToListAsync();

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> GetInvoiceStatusCount(Guid? UserID, string? UserCode)
        {
            try
            {
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                IQueryable<P_INV_HEADER_DETAIL> result = null;

                if (UserID.HasValue)
                {
                    var cgs = await (from tb in _dbContext.Users
                                     join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                     join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                     join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                     where tb.UserID == UserID
                                     select tb3).Distinct().ToListAsync();
                    if (cgs.Any())
                    {
                        result = from tb in _dbContext.P_INV_HEADER_DETAIL
                                 join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                 join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                 join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                 join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                 where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE
                                 select tb;
                    }
                    else
                    {
                        result = from tb in _dbContext.P_INV_HEADER_DETAIL
                                 join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                 join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                 join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                 where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE
                                 select tb;
                    }
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where tb.CUSTOMER == UserCode && tb.IS_ACTIVE
                             select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode must be provided.");
                }

                var query = await result
                    .Select(tb => new
                    {
                        tb.HEADER_ID,
                        tb.INV_NO,
                        tb.STATUS
                    }).ToListAsync();
                InvoiceStatusCount.TotalInvoices = query.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in query
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in query
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in query
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                InvoiceStatusCount.SavedInvoices = (from tb in query
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                return InvoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> GetDeliveryCount(Guid? UserID, string? UserCode)
        {
            try
            {
                DeliveryCount deliveryCount = new DeliveryCount();
                IQueryable<P_INV_HEADER_DETAIL> result = null;

                if (UserID.HasValue)
                {
                    var cgs = await (from tb in _dbContext.Users
                                     join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                     join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                     join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                     where tb.UserID == UserID
                                     select tb3).Distinct().ToListAsync();
                    if (cgs.Any())
                    {
                        result = from tb in _dbContext.P_INV_HEADER_DETAIL
                                 join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                 join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                 join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                 join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                 where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                 tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed")
                                 select tb;
                    }
                    else
                    {
                        result = from tb in _dbContext.P_INV_HEADER_DETAIL
                                 join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                 join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                 join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                 where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                 tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed")
                                 select tb;
                    }
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where tb.CUSTOMER == UserCode && tb.ACTUAL_DELIVERY_DATE.HasValue && tb.STATUS.ToLower().Contains("confirmed") && tb.IS_ACTIVE
                             select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode must be provided.");
                }
                var query = await result
                      .Select(tb => new
                      {
                          tb.HEADER_ID,
                          tb.INV_NO,
                          tb.ACTUAL_DELIVERY_DATE,
                          tb.PROPOSED_DELIVERY_DATE
                      }).ToListAsync();
                deliveryCount.TotalDelivery = query.Count;
                deliveryCount.InLineDelivery = (from tb in query
                                                where (!tb.PROPOSED_DELIVERY_DATE.HasValue || tb.ACTUAL_DELIVERY_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.INV_NO).Count();
                deliveryCount.DelayedDelivery = (from tb in query
                                                 where (tb.PROPOSED_DELIVERY_DATE.HasValue && tb.ACTUAL_DELIVERY_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.INV_NO).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> FilterInvoiceStatusCount(Guid? UserID, string? UserCode, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                InvoiceStatusCount InvoiceStatusCount = new InvoiceStatusCount();
                IQueryable<P_INV_HEADER_DETAIL> result = null;

                if (UserID.HasValue)
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                             join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                             join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                             where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                             (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                             select tb;
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                             select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode must be provided.");
                }
                var query = await result
                     .Select(tb => new
                     {
                         tb.HEADER_ID,
                         tb.INV_NO,
                         tb.STATUS
                     }).ToListAsync();
                InvoiceStatusCount.TotalInvoices = query.Count;
                InvoiceStatusCount.ConfirmedInvoices = (from tb in query
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                InvoiceStatusCount.PartiallyConfirmedInvoices = (from tb in query
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                InvoiceStatusCount.SavedInvoices = (from tb in query
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                InvoiceStatusCount.PendingInvoices = (from tb in query
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                return InvoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> FilterDeliveryCount(Guid? UserID, string? UserCode, string Organization = null, string Division = null, string Plant = null, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            try
            {
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                DeliveryCount deliveryCount = new DeliveryCount();
                IQueryable<P_INV_HEADER_DETAIL> result = null;

                if (UserID.HasValue)
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                             join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                             join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                             where tb3.UserID == UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                             tb.ACTUAL_DELIVERY_DATE.HasValue &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                             (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) &&
                             (!isPlant || tb.PLANT == Plant) && tb.STATUS.ToLower().Contains("confirmed")
                             select tb;
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where tb.CUSTOMER == UserCode && tb.IS_ACTIVE &&
                             (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                             (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date)) &&
                             tb.STATUS.ToLower() != "open"
                             select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode must be provided.");
                }

                var query = await result
                    .Select(tb => new
                    {
                        tb.HEADER_ID,
                        tb.INV_NO,
                        tb.ACTUAL_DELIVERY_DATE,
                        tb.PROPOSED_DELIVERY_DATE,
                        tb.VEHICLE_REPORTED_DATE
                    }).ToListAsync();

                deliveryCount.TotalDelivery = query.Count;
                deliveryCount.InLineDelivery = (from tb in query
                                                where (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                select tb.HEADER_ID).Count();
                deliveryCount.DelayedDelivery = (from tb in query
                                                 where (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                                 select tb.HEADER_ID).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> GetInvoiceheaderDetails(Guid? UserID, string? UserCode)
        {
            try
            {
                IQueryable<P_INV_HEADER_DETAIL> result = null;
                if (UserID.HasValue)
                {
                    var userResult = (from td in _dbContext.Users
                                      select td).Where(x => x.UserID == UserID).FirstOrDefault();
                    var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                    bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                    var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                    bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where (isAllOrganization || Organizations.Any(y => y == tb.ORGANIZATION)) && (tb.IS_ACTIVE) && (isAllPlant || Plants.Any(y => y == tb.PLANT))
                             orderby tb.HEADER_ID descending
                             select tb;
                }
                else if (!string.IsNullOrEmpty(UserCode))
                {
                    result = from tb in _dbContext.P_INV_HEADER_DETAIL
                             where tb.CUSTOMER == UserCode && tb.IS_ACTIVE
                             orderby tb.HEADER_ID descending
                             select tb;
                }
                else
                {
                    throw new Exception("Either UserID or UserCode should be provided.");
                }

                var query = await result
                            .Select(tb => new InvoiceHeaderDetails
                            {
                                HEADER_ID = tb.HEADER_ID,
                                INV_NO = tb.INV_NO,
                                ODIN = tb.ODIN,
                                INV_DATE = tb.INV_DATE,
                                INV_TYPE = tb.INV_TYPE,
                                PLANT = tb.PLANT,
                                VEHICLE_NO = tb.VEHICLE_NO,
                                PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                STATUS = tb.STATUS
                            }).Take(5).ToListAsync();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

    }
}
