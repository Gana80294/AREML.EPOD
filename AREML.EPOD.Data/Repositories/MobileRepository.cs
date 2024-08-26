using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Data.Logging;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class MobileRepository : IMobileRepository
    {
        private AuthContext _dbContext;

        public MobileRepository(AuthContext context)
        {
            this._dbContext = context;
        }


        public async Task<InvoiceStatusCount> FilterInvoicesStatusCount(FilterClass filterClass)
        {
            try
            {
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                int num = (filterClass.CurrentPage - 1) * filterClass.Records;
                int records = filterClass.Records;
                InvoiceStatusCount invoiceStatusCount = new InvoiceStatusCount();
                IQueryable<CustomerGroup> queryable = (from tb in _dbContext.Users
                                                       join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                                       join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                                       join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                                       where tb.UserID == filterClass.UserID
                                                       select tb3).Distinct();
                if (queryable.Count() > 0)
                {
                    var list = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID &&
                                      (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) 
                                      && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date))
                                      && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) &&
                                      (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) &&
                                      (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) &&
                                      (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) 
                                      && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                      select new { tb.HEADER_ID, tb.INV_NO, tb.INV_DATE, tb.STATUS }).ToListAsync();
                    invoiceStatusCount.TotalInvoices = list.Count;
                    invoiceStatusCount.ConfirmedInvoices = (from tb in list
                                                            where tb.STATUS.ToLower() == "confirmed"
                                                            select tb.INV_NO).Count();
                    invoiceStatusCount.PartiallyConfirmedInvoices = (from tb in list
                                                                     where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                     select tb.INV_NO).Count();
                    invoiceStatusCount.SavedInvoices = (from tb in list
                                                        where tb.STATUS.ToLower() == "saved"
                                                        select tb.INV_NO).Count();
                    invoiceStatusCount.PendingInvoices = (from tb in list
                                                          where tb.STATUS.ToLower() == "open"
                                                          select tb.INV_NO).Count();
                    return invoiceStatusCount;
                }
                var list2 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                   select new { tb.HEADER_ID, tb.INV_NO, tb.INV_DATE, tb.STATUS }).ToListAsync();
                invoiceStatusCount.TotalInvoices = list2.Count;
                invoiceStatusCount.ConfirmedInvoices = (from tb in list2
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                invoiceStatusCount.PartiallyConfirmedInvoices = (from tb in list2
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                invoiceStatusCount.SavedInvoices = (from tb in list2
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                invoiceStatusCount.PendingInvoices = (from tb in list2
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                return invoiceStatusCount;
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
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                DeliveryCount deliveryCount = new DeliveryCount();
                IQueryable<CustomerGroup> queryable = (from tb in _dbContext.Users
                                                       join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                                       join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                                       join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                                       where tb.UserID == filterClass.UserID
                                                       select tb3).Distinct();
                if (queryable.Count() > 0)
                {
                    var list = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && tb.STATUS.ToLower() != "open" && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                      select new { tb.HEADER_ID, tb.PROPOSED_DELIVERY_DATE, tb.VEHICLE_REPORTED_DATE, tb.STATUS }).ToListAsync();
                    deliveryCount.TotalDelivery = list.Count;
                    deliveryCount.InLineDelivery = (from tb in list
                                                    where !tb.PROPOSED_DELIVERY_DATE.HasValue || !tb.VEHICLE_REPORTED_DATE.HasValue || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                    select tb.HEADER_ID).Count();
                    deliveryCount.DelayedDelivery = (from tb in list
                                                     where tb.PROPOSED_DELIVERY_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                     select tb.HEADER_ID).Count();
                    return deliveryCount;
                }
                var list2 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && tb.STATUS.ToLower() != "open" && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                   select new { tb.HEADER_ID, tb.PROPOSED_DELIVERY_DATE, tb.VEHICLE_REPORTED_DATE, tb.STATUS }).ToListAsync();
                deliveryCount.TotalDelivery = list2.Count;
                deliveryCount.InLineDelivery = (from tb in list2
                                                where !tb.PROPOSED_DELIVERY_DATE.HasValue || !tb.VEHICLE_REPORTED_DATE.HasValue || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                select tb.HEADER_ID).Count();
                deliveryCount.DelayedDelivery = (from tb in list2
                                                 where tb.PROPOSED_DELIVERY_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                 select tb.HEADER_ID).Count();
                return deliveryCount;
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
                List<int> list = new List<int>();
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                DeliveryCount deliveryCount = new DeliveryCount();
                IQueryable<CustomerGroup> queryable = (from tb in _dbContext.Users
                                                       join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                                       join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                                       join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                                       where tb.UserID == filterClass.UserID
                                                       select tb3).Distinct();
                if (queryable.Count() > 0)
                {
                    var list2 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                       join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                       join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                       join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                       join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                       where tb3.UserID == filterClass.UserID && tb.IS_ACTIVE && tb2.UserID == tb3.UserID && tb.STATUS.ToLower() == "open" && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                       select new { tb.PROPOSED_DELIVERY_DATE }).ToListAsync();
                    list.Add(list2.Count);
                    list.Add(list2.Where(t =>
                    {
                        DateTime date4 = DateTime.Now.Date;
                        DateTime? pROPOSED_DELIVERY_DATE4 = t.PROPOSED_DELIVERY_DATE;
                        return date4 <= pROPOSED_DELIVERY_DATE4 || !t.PROPOSED_DELIVERY_DATE.HasValue;
                    }).Count());
                    list.Add(list2.Where(t =>
                    {
                        DateTime date3 = DateTime.Now.Date;
                        DateTime? pROPOSED_DELIVERY_DATE3 = t.PROPOSED_DELIVERY_DATE;
                        return date3 > pROPOSED_DELIVERY_DATE3 && t.PROPOSED_DELIVERY_DATE.HasValue;
                    }).Count());
                    return list;
                }
                var list3 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && tb.STATUS.ToLower() == "open" && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k == tb.CUSTOMER_GROUP_DESC)) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName))
                                   select new { tb.PROPOSED_DELIVERY_DATE }).ToListAsync();
                list.Add(list3.Count);
                list.Add(list3.Where(t =>
                {
                    DateTime date2 = DateTime.Now.Date;
                    DateTime? pROPOSED_DELIVERY_DATE2 = t.PROPOSED_DELIVERY_DATE;
                    return date2 <= pROPOSED_DELIVERY_DATE2 || !t.PROPOSED_DELIVERY_DATE.HasValue;
                }).Count());
                list.Add(list3.Where(t =>
                {
                    DateTime date = DateTime.Now.Date;
                    DateTime? pROPOSED_DELIVERY_DATE = t.PROPOSED_DELIVERY_DATE;
                    return date > pROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE.HasValue;
                }).Count());
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<InvoiceStatusCount> FilterInvoiceStatusCountByUser(FilterClass filterClass)
        {
            try
            {
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                InvoiceStatusCount invoiceStatusCount = new InvoiceStatusCount();
                var list = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date))
                                  select new { tb.HEADER_ID, tb.INV_NO, tb.STATUS }).ToListAsync();
                invoiceStatusCount.TotalInvoices = list.Count;
                invoiceStatusCount.ConfirmedInvoices = (from tb in list
                                                        where tb.STATUS.ToLower() == "confirmed"
                                                        select tb.INV_NO).Count();
                invoiceStatusCount.PartiallyConfirmedInvoices = (from tb in list
                                                                 where tb.STATUS.ToLower() == "partiallyconfirmed"
                                                                 select tb.INV_NO).Count();
                invoiceStatusCount.SavedInvoices = (from tb in list
                                                    where tb.STATUS.ToLower() == "saved"
                                                    select tb.INV_NO).Count();
                invoiceStatusCount.PendingInvoices = (from tb in list
                                                      where tb.STATUS.ToLower() == "open"
                                                      select tb.INV_NO).Count();
                return invoiceStatusCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeliveryCount> FilterDeliveryCountByUser(FilterClass filterClass)
        {
            try
            {
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                DeliveryCount deliveryCount = new DeliveryCount();
                var list = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && 
                                  tb.IS_ACTIVE && 
                                  (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && 
                                  (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) &&
                                  tb.STATUS.ToLower() != "open"
                                  select new 
                                  { 
                                      tb.HEADER_ID, 
                                      tb.INV_NO,
                                      tb.ACTUAL_DELIVERY_DATE,
                                      tb.PROPOSED_DELIVERY_DATE,
                                      tb.VEHICLE_REPORTED_DATE
                                  }).ToListAsync();
                deliveryCount.TotalDelivery = list.Count;
                deliveryCount.InLineDelivery = (from tb in list
                                                where !tb.PROPOSED_DELIVERY_DATE.HasValue || !tb.VEHICLE_REPORTED_DATE.HasValue || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                select tb.INV_NO).Count();
                deliveryCount.DelayedDelivery = (from tb in list
                                                 where tb.PROPOSED_DELIVERY_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.HasValue && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date
                                                 select tb.INV_NO).Count();
                return deliveryCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<int>> FilterLeadTimeCountByUser(FilterClass filterClass)
        {
            try
            {
                List<int> list = new List<int>();
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                InvoiceStatusCount invoiceStatusCount = new InvoiceStatusCount();
                var list2 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                   where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && tb.STATUS.ToLower() == "open"
                                   select new { tb.PROPOSED_DELIVERY_DATE }).ToListAsync();
                list.Add(list2.Count);
                list.Add(list2.Where(t =>
                {
                    int result2;
                    if (t.PROPOSED_DELIVERY_DATE.HasValue)
                    {
                        DateTime date2 = DateTime.Now.Date;
                        DateTime? pROPOSED_DELIVERY_DATE2 = t.PROPOSED_DELIVERY_DATE;
                        result2 = ((date2 <= pROPOSED_DELIVERY_DATE2) ? 1 : 0);
                    }
                    else
                    {
                        result2 = 1;
                    }
                    return (byte)result2 != 0;
                }).Count());
                list.Add(list2.Where(t =>
                {
                    int result;
                    if (t.PROPOSED_DELIVERY_DATE.HasValue)
                    {
                        DateTime date = DateTime.Now.Date;
                        DateTime? pROPOSED_DELIVERY_DATE = t.PROPOSED_DELIVERY_DATE;
                        result = ((date > pROPOSED_DELIVERY_DATE) ? 1 : 0);
                    }
                    else
                    {
                        result = 0;
                    }
                    return (byte)result != 0;
                }).Count());
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterInvoices(FilterClass filterClass)
        {
            try
            {
                int count = (filterClass.CurrentPage - 1) * filterClass.Records;
                int records = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                IQueryable<CustomerGroup> queryable = (from tb in _dbContext.Users
                                                       join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                                                       join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                                                       join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                                                       where tb.UserID == filterClass.UserID
                                                       select tb3).Distinct();
                if (queryable.Count() > 0)
                {
                    List<InvoiceHeaderDetails> list = new List<InvoiceHeaderDetails>();
                    DateTime currDate2 = DateTime.Now.Date;
                    if (filterClass.LeadTime.Count == 0 && filterClass.Delivery.Count == 0)
                    {
                        list = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber))
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open" && (currDate2 <= tb.PROPOSED_DELIVERY_DATE || tb.PROPOSED_DELIVERY_DATE == null)
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open" && currDate2 > tb.PROPOSED_DELIVERY_DATE && tb.PROPOSED_DELIVERY_DATE != null
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open"
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open" && (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date <= (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open" && tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date > (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                    {
                        list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                      join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                      join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                      join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                      join tb4 in queryable on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                      where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open"
                                      orderby tb.HEADER_ID
                                      select new InvoiceHeaderDetails
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
                                          FWD_AGENT = tb.FWD_AGENT,
                                          EWAYBILL_NO = tb.EWAYBILL_NO,
                                          DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                          TRACKING_LINK = tb.TRACKING_LINK,
                                          TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                          TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                          DELIVERY_DATE = tb.DELIVERY_DATE,
                                          DELIVERY_TIME = tb.DELIVERY_TIME
                                      }).Skip(count).Take(records).ToListAsync();
                    }
                    foreach (InvoiceHeaderDetails res2 in list)
                    {
                        var anon = (from x in (IQueryable<P_INV_ATTACHMENT>)_dbContext.P_INV_ATTACHMENT
                                    where x.HEADER_ID == res2.HEADER_ID
                                    select x into k
                                    select new { k.ATTACHMENT_ID, k.ATTACHMENT_NAME }).FirstOrDefault();
                        if (anon != null)
                        {
                            res2.ATTACHMENT_ID = anon.ATTACHMENT_ID;
                            res2.ATTACHMENT_NAME = anon.ATTACHMENT_NAME;
                        }
                    }
                    return list;
                }
                List<InvoiceHeaderDetails> list2 = new List<InvoiceHeaderDetails>();
                DateTime currDate = DateTime.Now.Date;
                if (filterClass.LeadTime.Count == 0 && filterClass.Delivery.Count == 0)
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber))
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open" && (currDate <= tb.PROPOSED_DELIVERY_DATE || tb.PROPOSED_DELIVERY_DATE == null)
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open" && currDate > tb.PROPOSED_DELIVERY_DATE && tb.PROPOSED_DELIVERY_DATE != null
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS == "Open"
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open" && (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date <= (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open" && tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date > (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                {
                    list2 = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                   join tb1 in (IEnumerable<UserOrganizationMap>)_dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                   join tb2 in (IEnumerable<UserPlantMap>)_dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                   join tb3 in (IEnumerable<User>)_dbContext.Users on tb1.UserID equals tb3.UserID
                                   where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isOrganization || filterClass.Organization.Any((string x) => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any((string x) => x == tb.DIVISION)) && (!isPlant || filterClass.PlantList.Any((string x) => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any((string k) => k.ToLower().Contains(tb.CUSTOMER_GROUP_DESC))) && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && tb.STATUS != "Open"
                                   orderby tb.HEADER_ID
                                   select new InvoiceHeaderDetails
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
                                       FWD_AGENT = tb.FWD_AGENT,
                                       EWAYBILL_NO = tb.EWAYBILL_NO,
                                       DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                       TRACKING_LINK = tb.TRACKING_LINK,
                                       TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                       TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                       DELIVERY_DATE = tb.DELIVERY_DATE,
                                       DELIVERY_TIME = tb.DELIVERY_TIME
                                   }).Skip(count).Take(records).ToListAsync();
                }
                foreach (InvoiceHeaderDetails res in list2)
                {
                    var anon2 = (from x in (IQueryable<P_INV_ATTACHMENT>)_dbContext.P_INV_ATTACHMENT
                                 where x.HEADER_ID == res.HEADER_ID
                                 select x into k
                                 select new { k.ATTACHMENT_ID, k.ATTACHMENT_NAME }).FirstOrDefault();
                    if (anon2 != null)
                    {
                        res.ATTACHMENT_ID = anon2.ATTACHMENT_ID;
                        res.ATTACHMENT_NAME = anon2.ATTACHMENT_NAME;
                    }
                }
                return list2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<InvoiceHeaderDetails>> FilterInvoicesByUser(FilterClass filterClass)
        {
            try
            {
                int count = (filterClass.CurrentPage - 1) * filterClass.Records;
                int records = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isLRNumber = !string.IsNullOrEmpty(filterClass.LRNumber);
                List<InvoiceHeaderDetails> list = new List<InvoiceHeaderDetails>();
                DateTime currDate = DateTime.Now.Date;
                if (filterClass.LeadTime.Count == 0 && filterClass.Delivery.Count == 0)
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber)
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS == "Open" && (currDate <= tb.PROPOSED_DELIVERY_DATE || tb.PROPOSED_DELIVERY_DATE == null)
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS == "Open" && currDate > tb.PROPOSED_DELIVERY_DATE && tb.PROPOSED_DELIVERY_DATE != null
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS == "Open"
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS != "Open" && (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date <= (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date)
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS != "Open" && tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && (DateTime?)tb.VEHICLE_REPORTED_DATE.Value.Date > (DateTime?)tb.PROPOSED_DELIVERY_DATE.Value.Date
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                {
                    list = await (from tb in (IQueryable<P_INV_HEADER_DETAIL>)_dbContext.P_INV_HEADER_DETAIL
                                  where tb.CUSTOMER == filterClass.UserCode && tb.IS_ACTIVE && (!isFromDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date >= (DateTime?)filterClass.StartDate.Value.Date)) && (!isEndDate || (tb.INV_DATE.HasValue && (DateTime?)tb.INV_DATE.Value.Date <= (DateTime?)filterClass.EndDate.Value.Date)) && (!isStatus || filterClass.Status.Any((string x) => x == tb.STATUS)) && (!isInvoiceNumber || tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber) && tb.STATUS != "Open"
                                  orderby tb.HEADER_ID
                                  select new InvoiceHeaderDetails
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
                                      FWD_AGENT = tb.FWD_AGENT,
                                      EWAYBILL_NO = tb.EWAYBILL_NO,
                                      DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                      TRACKING_LINK = tb.TRACKING_LINK,
                                      TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                      TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                      DELIVERY_DATE = tb.DELIVERY_DATE,
                                      DELIVERY_TIME = tb.DELIVERY_TIME
                                  }).Skip(count).Take(records).ToListAsync();
                }
                foreach (InvoiceHeaderDetails res in list)
                {
                    var anon = (from x in (IQueryable<P_INV_ATTACHMENT>)_dbContext.P_INV_ATTACHMENT
                                where x.HEADER_ID == res.HEADER_ID
                                select x into k
                                select new { k.ATTACHMENT_ID, k.ATTACHMENT_NAME }).FirstOrDefault();
                    if (anon != null)
                    {
                        res.ATTACHMENT_ID = anon.ATTACHMENT_ID;
                        res.ATTACHMENT_NAME = anon.ATTACHMENT_NAME;
                    }
                }
                return list.Skip(count).Take(records).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> DowloandPODDocument(int HeaderID, int AttachmentID)
        {
            try
            {
                P_INV_ATTACHMENT p_INV_ATTACHMENT = ((IQueryable<P_INV_ATTACHMENT>)_dbContext.P_INV_ATTACHMENT).Where((P_INV_ATTACHMENT x) => x.HEADER_ID == HeaderID && x.ATTACHMENT_ID == AttachmentID).FirstOrDefault();
                if (p_INV_ATTACHMENT != null && p_INV_ATTACHMENT.ATTACHMENT_FILE.Length != 0)
                {
                    byte[] aTTACHMENT_FILE = p_INV_ATTACHMENT.ATTACHMENT_FILE;
                    return Convert.ToBase64String(aTTACHMENT_FILE);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<MobileVersion> GetMobileAppVersion()
        {
            try
            {
                return await this._dbContext.MobileVersions.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
                throw ex;
            }
        }
        public async Task<MobileVersion> UpdateMobileVersion(string version)
        {
            try
            {
                var oldVersion = await this._dbContext.MobileVersions.FirstOrDefaultAsync();
                oldVersion.VersionCode = version;
                _dbContext.SaveChanges();
                return oldVersion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
