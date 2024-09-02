using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private AuthContext _dbContext;
        private ExcelHelper _excelHelper;
        private readonly NetworkCredentials _networkCredentials;


        public ReportRepository(AuthContext context, ExcelHelper excel,IConfiguration configuration)
        {
            this._dbContext = context;
            this._excelHelper = excel;
            _networkCredentials = configuration.GetSection("NetworkCredentials").Get<NetworkCredentials>();

        }
        

        public async Task<List<ReportInvoice>> GetPendingInvoiceDetails(Guid UserID)
        {
            try
            {
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                    join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                    join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                    where tb4.UserID == UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                    tb.STATUS.ToLower() == "open"
                                    select new ReportInvoice
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        ITEM_ID = tb1.ITEM_ID,
                                        ITEM_NO = tb1.ITEM_NO,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        FWD_AGENT = tb.FWD_AGENT,
                                        CARRIER = tb.CARRIER,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        EWAYBILL_NO = tb.EWAYBILL_NO,
                                        EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                        FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                        MATERIAL_CODE = tb1.MATERIAL_CODE,
                                        MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                        QUANTITY = tb1.QUANTITY,
                                        RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                        QUANTITY_UOM = tb1.QUANTITY_UOM,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        POD_UPLOADE_STATUS = tb.STATUS,
                                        TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                        CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                        STATUS = tb.STATUS,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME
                                    }).OrderBy(x => x.INV_NO).ToListAsync();
                foreach (var res in result)
                {
                    var att = _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == res.HEADER_ID).FirstOrDefault();
                    if (att != null)
                    {
                        res.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        res.ATTACHMENT_NAME = att.ATTACHMENT_NAME;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ReportInvoice>> GetFilteredInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            var status = Status.ToLower();
            try
            {
                bool isStatus = status != "all";
                if (status == "confirmed")
                {
                    StartDate = DateTime.Now.AddDays(-30);
                    EndDate = DateTime.Now;
                }
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(InvoiceNumber);
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                bool isCustomerName = !string.IsNullOrEmpty(CustomerName);
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                    join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                    join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                    where tb4.UserID == UserID && tb3.UserID == tb4.UserID &&
                                    (!isStatus || tb.STATUS.ToLower() == status) && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    && (!isInvoiceNumber || tb.ODIN == InvoiceNumber)
                                    && (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(CustomerName.ToLower()))
                                    select new ReportInvoice
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        ITEM_ID = tb1.ITEM_ID,
                                        ITEM_NO = tb1.ITEM_NO,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        ORGANIZATION = tb.ORGANIZATION,
                                        DIVISION = tb.DIVISION,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        FWD_AGENT = tb.FWD_AGENT,
                                        CARRIER = tb.CARRIER,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        EWAYBILL_NO = tb.EWAYBILL_NO,
                                        EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                        FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                        MATERIAL_CODE = tb1.MATERIAL_CODE,
                                        MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                        QUANTITY = tb1.QUANTITY,
                                        RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                        QUANTITY_UOM = tb1.QUANTITY_UOM,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        POD_UPLOADE_STATUS = tb.STATUS,
                                        TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                        CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME
                                    }).OrderBy(x => x.INV_NO).ToListAsync();
                foreach (var res in result)
                {
                    var att = _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == res.HEADER_ID).FirstOrDefault();
                    if (att != null)
                    {
                        res.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        res.ATTACHMENT_NAME = att.ATTACHMENT_NAME;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ReportInvoice>> GetFilteredInvoiceDetails(Guid UserID, int CurrentPage, int Records, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null, string CustomerGroup = null)
        {
            var SkipValue = (CurrentPage - 1) * Records;
            var TakeValue = Records;
            var status = Status.ToLower();
            try
            {
                bool isStatus = status != "all";
                if (status == "confirmed")
                {
                    StartDate = DateTime.Now.AddDays(-30);
                    EndDate = DateTime.Now;
                }
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(InvoiceNumber);
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                bool isCustomerName = !string.IsNullOrEmpty(CustomerName);
                bool isCustomerGroup = !string.IsNullOrEmpty(CustomerGroup);

                var result1 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                     join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                     join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                     join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                     where tb4.UserID == UserID && tb3.UserID == tb4.UserID &&
                                     (!isStatus || tb.STATUS.ToLower() == status) && tb.IS_ACTIVE &&
                                     (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                     (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                     && (!isInvoiceNumber || tb.ODIN.Contains(InvoiceNumber))
                                     && (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                     && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(CustomerName.ToLower()))
                                     && (!isCustomerGroup || tb.CUSTOMER_GROUP.ToLower().Contains(CustomerGroup.ToLower()))
                                     orderby tb.HEADER_ID
                                     select tb).Skip(SkipValue).Take(TakeValue).ToListAsync();

                var result = (from tb in result1
                              join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                              select new ReportInvoice
                              {
                                  HEADER_ID = tb.HEADER_ID,
                                  INV_NO = tb.INV_NO,
                                  ODIN = tb.ODIN,
                                  ITEM_ID = tb1.ITEM_ID,
                                  ITEM_NO = tb1.ITEM_NO,
                                  INV_DATE = tb.INV_DATE,
                                  INV_TYPE = tb.INV_TYPE,
                                  CUSTOMER = tb.CUSTOMER,
                                  CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                  ORGANIZATION = tb.ORGANIZATION,
                                  CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                  DIVISION = tb.DIVISION,
                                  LR_NO = tb.LR_NO,
                                  LR_DATE = tb.LR_DATE,
                                  VEHICLE_NO = tb.VEHICLE_NO,
                                  FWD_AGENT = tb.FWD_AGENT,
                                  CARRIER = tb.CARRIER,
                                  VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                  EWAYBILL_NO = tb.EWAYBILL_NO,
                                  EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                  FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                  FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                  MATERIAL_CODE = tb1.MATERIAL_CODE,
                                  MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                  QUANTITY = tb1.QUANTITY,
                                  RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                  QUANTITY_UOM = tb1.QUANTITY_UOM,
                                  PLANT = tb.PLANT,
                                  PLANT_NAME = tb.PLANT_NAME,
                                  OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                  OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                  ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                  PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                  VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                  ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                  POD_UPLOADE_STATUS = tb.STATUS,
                                  TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                  CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                  STATUS = tb.STATUS,
                                  TRACKING_LINK = tb.TRACKING_LINK,
                                  DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                  TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                  TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                  DELIVERY_DATE = tb.DELIVERY_DATE,
                                  DELIVERY_TIME = tb.DELIVERY_TIME
                              }).OrderBy(x => x.INV_NO).ToList();


                foreach (var res in result)
                {
                    var att = _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == res.HEADER_ID).FirstOrDefault();
                    if (att != null)
                    {
                        res.ATTACHMENT_ID = att.ATTACHMENT_ID;
                        res.ATTACHMENT_NAME = att.ATTACHMENT_NAME;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<ReportInvoice>> GetFilteredInvoiceDetails(FilterClass filterClass)
        {
            var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
            var TakeValue = filterClass.Records;
            var status = filterClass.Status;

            try
            {
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;
                var cgs = (from tb in _dbContext.Users
                           join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                           join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                           join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                           where tb.UserID == filterClass.UserID
                           select tb3).Distinct();

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
                    var result1 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                         join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                         join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                         join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                         join tb5 in cgs on tb.CUSTOMER_GROUP_DESC equals tb5.CustomerGroupCode
                                         join tb8 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb8.HEADER_ID
                                         where tb4.UserID == filterClass.UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                         (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) &&
                                         (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                         (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                         && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                         && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION)) &&
                                         (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         orderby tb.HEADER_ID
                                         select new ReportInvoice
                                         {
                                             HEADER_ID = tb.HEADER_ID,
                                             INV_NO = tb.INV_NO,
                                             ODIN = tb.ODIN,
                                             ITEM_ID = tb8.ITEM_ID,
                                             ITEM_NO = tb8.ITEM_NO,
                                             INV_DATE = tb.INV_DATE,
                                             INV_TYPE = tb.INV_TYPE,
                                             CUSTOMER = tb.CUSTOMER,
                                             CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                             CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                             ORGANIZATION = tb.ORGANIZATION,
                                             DIVISION = tb.DIVISION,
                                             LR_NO = tb.LR_NO,
                                             LR_DATE = tb.LR_DATE,
                                             VEHICLE_NO = tb.VEHICLE_NO,
                                             FWD_AGENT = tb.FWD_AGENT,
                                             CARRIER = tb.CARRIER,
                                             VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                             EWAYBILL_NO = tb.EWAYBILL_NO,
                                             EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                             FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                             FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                             MATERIAL_CODE = tb8.MATERIAL_CODE,
                                             MATERIAL_DESCRIPTION = tb8.MATERIAL_DESCRIPTION,
                                             QUANTITY = tb8.QUANTITY,
                                             RECEIVED_QUANTITY = tb8.RECEIVED_QUANTITY,
                                             QUANTITY_UOM = tb8.QUANTITY_UOM,
                                             PLANT = tb.PLANT,
                                             PLANT_NAME = tb.PLANT_NAME,
                                             OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                             OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                             ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                             PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                             VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                             ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                             POD_UPLOADE_STATUS = tb.STATUS,
                                             TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                             CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                             STATUS = tb.STATUS,
                                             PLANT_CODE = tb.PLANT_CODE,
                                             SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                             GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                             CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                             CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                             REMARKS = tb8.REMARKS,
                                             DISTANCE = tb.DISTANCE,
                                             ITEM_WEIGHT = tb8.ITEM_WEIGHT,
                                             TRACKING_LINK = tb.TRACKING_LINK,
                                             DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                             TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                             TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                             DELIVERY_DATE = tb.DELIVERY_DATE,
                                             DELIVERY_TIME = tb.DELIVERY_TIME
                                         }).Skip(SkipValue).Take(TakeValue).OrderBy(x => x.INV_NO).ToListAsync();
                    var PDDList = new List<ReportInvoice>();
                    if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result1.Where(t => (DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result1.Where(t => (DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                    {
                        PDDList = result1.Where(t => t.STATUS.ToLower() == "open").ToList();
                    }

                    var DeliveryList = new List<ReportInvoice>();
                    if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result1.Where(tb => (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result1.Where(tb => (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                    {
                        DeliveryList = result1.Where(t => t.STATUS.ToLower() != "open").ToList();
                    }

                    if (filterClass.Delivery.Count > 0 || filterClass.LeadTime.Count > 0)
                    {
                        result1 = PDDList.Concat(DeliveryList).ToList();
                    }

                    foreach (var res in result1)
                    {
                        var att = _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == res.HEADER_ID).Select(k => new { k.ATTACHMENT_ID, k.ATTACHMENT_NAME }).FirstOrDefault();
                        if (att != null)
                        {
                            res.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            res.ATTACHMENT_NAME = att.ATTACHMENT_NAME;
                        }
                    }
                    return result1;
                }
                else
                {
                    var result1 = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                         join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                         join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                         join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID

                                         join tb8 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb8.HEADER_ID

                                         where tb4.UserID == filterClass.UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                         (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) &&

                                         (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                         (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                         && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                         && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                         && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                         && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                         && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                         && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                         orderby tb.HEADER_ID
                                         select new ReportInvoice
                                         {
                                             HEADER_ID = tb.HEADER_ID,
                                             INV_NO = tb.INV_NO,
                                             ODIN = tb.ODIN,
                                             ITEM_ID = tb8.ITEM_ID,
                                             ITEM_NO = tb8.ITEM_NO,
                                             INV_DATE = tb.INV_DATE,
                                             INV_TYPE = tb.INV_TYPE,
                                             CUSTOMER = tb.CUSTOMER,
                                             CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                             CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                             ORGANIZATION = tb.ORGANIZATION,
                                             DIVISION = tb.DIVISION,
                                             LR_NO = tb.LR_NO,
                                             LR_DATE = tb.LR_DATE,
                                             VEHICLE_NO = tb.VEHICLE_NO,
                                             FWD_AGENT = tb.FWD_AGENT,
                                             CARRIER = tb.CARRIER,
                                             VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                             EWAYBILL_NO = tb.EWAYBILL_NO,
                                             EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                             FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                             FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                             MATERIAL_CODE = tb8.MATERIAL_CODE,
                                             MATERIAL_DESCRIPTION = tb8.MATERIAL_DESCRIPTION,
                                             QUANTITY = tb8.QUANTITY,
                                             RECEIVED_QUANTITY = tb8.RECEIVED_QUANTITY,
                                             QUANTITY_UOM = tb8.QUANTITY_UOM,
                                             PLANT = tb.PLANT,
                                             PLANT_NAME = tb.PLANT_NAME,
                                             OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                             OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                             ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                             PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                             VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                             ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                             POD_UPLOADE_STATUS = tb.STATUS,
                                             TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                             CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                             STATUS = tb.STATUS,
                                             PLANT_CODE = tb.PLANT_CODE,
                                             SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                             GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                             CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                             CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                             REMARKS = tb8.REMARKS,
                                             DISTANCE = tb.DISTANCE,
                                             ITEM_WEIGHT = tb8.ITEM_WEIGHT,
                                             TRACKING_LINK = tb.TRACKING_LINK,
                                             DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                             TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                             TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                             DELIVERY_DATE = tb.DELIVERY_DATE,
                                             DELIVERY_TIME = tb.DELIVERY_TIME
                                         }).Skip(SkipValue).Take(TakeValue).OrderBy(x => x.INV_NO).ToListAsync();

                    var PDDList = new List<ReportInvoice>();
                    if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result1.Where(t => (DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result1.Where(t => (DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                    {
                        PDDList = result1.Where(t => t.STATUS.ToLower() == "open").ToList();
                    }

                    var DeliveryList = new List<ReportInvoice>();
                    if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result1.Where(tb => (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result1.Where(tb => (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                    {
                        DeliveryList = result1.Where(t => t.STATUS.ToLower() != "open").ToList();
                    }

                    if (filterClass.Delivery.Count > 0 || filterClass.LeadTime.Count > 0)
                    {
                        result1 = PDDList.Concat(DeliveryList).ToList();
                    }

                    foreach (var res in result1)
                    {
                        var att = _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == res.HEADER_ID).Select(k => new { k.ATTACHMENT_ID, k.ATTACHMENT_NAME }).FirstOrDefault();
                        if (att != null)
                        {
                            res.ATTACHMENT_ID = att.ATTACHMENT_ID;
                            res.ATTACHMENT_NAME = att.ATTACHMENT_NAME;
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

        public async Task<byte[]> DownloadInvoiceDetails(Guid UserID, string Status, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            CreateTempFolder();
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

            var status = Status.ToLower();
            var today = DateTime.Now;
            var lastday = DateTime.Now.AddDays(-30);

            try
            {
                bool isStatus = status != "all";
                if (status == "confirmed")
                {
                    if (StartDate.HasValue)
                    {
                        if (StartDate.Value.Date < lastday.Date)
                        {
                            StartDate = lastday;
                        }
                    }
                    if (EndDate.HasValue)
                    {
                        if (EndDate.Value.Date > today.Date)
                        {
                            EndDate = today;
                        }
                    }
                }
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(InvoiceNumber);
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                bool isCustomerName = !string.IsNullOrEmpty(CustomerName);

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                    join tb5 in _dbContext.Organizations on tb2.OrganizationCode equals tb5.OrganizationCode
                                    join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                    join tb6 in _dbContext.Plants on tb3.PlantCode equals tb6.PlantCode
                                    join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                    where tb4.UserID == UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                    (!isStatus || tb.STATUS.ToLower() == status) &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    && (!isInvoiceNumber || tb.ODIN == InvoiceNumber)
                                    && (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(CustomerName.ToLower()))
                                    select new ReportInvoice
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        ITEM_ID = tb1.ITEM_ID,
                                        ITEM_NO = tb1.ITEM_NO,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        ORGANIZATION = tb5.Description,
                                        DIVISION = tb.DIVISION,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        FWD_AGENT = tb.FWD_AGENT,
                                        CARRIER = tb.CARRIER,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        EWAYBILL_NO = tb.EWAYBILL_NO,
                                        EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                        FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                        MATERIAL_CODE = tb1.MATERIAL_CODE,
                                        MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                        QUANTITY = tb1.QUANTITY,
                                        RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                        QUANTITY_UOM = tb1.QUANTITY_UOM,
                                        PLANT = tb6.Description,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        POD_UPLOADE_STATUS = tb.STATUS,
                                        TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                        CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                        STATUS = tb.STATUS,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME
                                    }).OrderBy(x => x.INV_NO).ToListAsync();


                using (var workbook = new XSSFWorkbook())
                {
                    ISheet sheet = _excelHelper.CreateNPOIworksheetReport(result, workbook);
                    using (var stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        return stream.ToArray();
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> DownloadInvoiceDetails(FilterClass filterClass)
        {
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            CreateTempFolder();
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

            var status = filterClass.Status;
            var today = DateTime.Now;
            var lastday = DateTime.Now.AddDays(-30);

            try
            {
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;


                var cgs = (from tb in _dbContext.Users
                           join tb1 in _dbContext.UserSalesGroupMaps on tb.UserID equals tb1.UserID
                           join tb2 in _dbContext.SLSGroupWithCustomerGroupMaps on tb1.SGID equals tb2.SGID
                           join tb3 in _dbContext.CustomerGroups on tb2.CGID equals tb3.CGID
                           where tb.UserID == filterClass.UserID
                           select tb3).Distinct();
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
                                        join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                        join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                        join tb5 in _dbContext.Organizations on tb2.OrganizationCode equals tb5.OrganizationCode
                                        join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                        join tb6 in _dbContext.Plants on tb3.PlantCode equals tb6.PlantCode
                                        join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                        join tb7 in cgs on tb.CUSTOMER_GROUP_DESC equals tb7.CustomerGroupCode
                                        where tb4.UserID == filterClass.UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                        (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        select new ReportInvoice
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            ITEM_ID = tb1.ITEM_ID,
                                            ITEM_NO = tb1.ITEM_NO,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            ORGANIZATION = tb5.Description,
                                            DIVISION = tb.DIVISION,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            FWD_AGENT = tb.FWD_AGENT,
                                            CARRIER = tb.CARRIER,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            EWAYBILL_NO = tb.EWAYBILL_NO,
                                            EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                            FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                            FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                            MATERIAL_CODE = tb1.MATERIAL_CODE,
                                            MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                            QUANTITY = tb1.QUANTITY,
                                            RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                            QUANTITY_UOM = tb1.QUANTITY_UOM,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                            OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                            ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            POD_UPLOADE_STATUS = tb.STATUS,
                                            TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                            CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                            STATUS = tb.STATUS,
                                            CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                            CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                            CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                            GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                            PLANT_CODE = tb.PLANT_CODE,
                                            SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                            REMARKS = tb1.REMARKS,
                                            DISTANCE = tb.DISTANCE,
                                            ITEM_WEIGHT = tb1.ITEM_WEIGHT,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME
                                        }).OrderBy(x => x.INV_NO).ToListAsync();
                    var PDDList = new List<ReportInvoice>();
                    if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result.Where(t => (DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result.Where(t => (DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                    {
                        PDDList = result.Where(t => t.STATUS.ToLower() == "open").ToList();
                    }

                    var DeliveryList = new List<ReportInvoice>();
                    if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result.Where(tb => (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result.Where(tb => (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                    {
                        DeliveryList = result.Where(t => t.STATUS.ToLower() != "open").ToList();
                    }

                    if (filterClass.Delivery.Count > 0 || filterClass.LeadTime.Count > 0)
                    {
                        result = PDDList.Concat(DeliveryList).ToList();
                    }

                    foreach (var header in result)
                    {
                        var sector = _dbContext.CustomerGroups.FirstOrDefault(t => t.CustomerGroupCode == header.CUSTOMER_GROUP_DESC);
                        if (sector != null)
                        {
                            header.SECTOR_DESCRIPTION = sector.Sector;
                        }
                    }
                    using (var workbook = new XSSFWorkbook())
                    {
                        ISheet sheet = _excelHelper.CreateNPOIworksheetReport(result, workbook);
                        using (var stream = new MemoryStream())
                        {
                            workbook.Write(stream);
                            return stream.ToArray();
                        }
                    }
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                        join tb2 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb2.OrganizationCode
                                        join tb5 in _dbContext.Organizations on tb2.OrganizationCode equals tb5.OrganizationCode
                                        join tb3 in _dbContext.UserPlantMaps on tb.PLANT equals tb3.PlantCode
                                        join tb6 in _dbContext.Plants on tb3.PlantCode equals tb6.PlantCode
                                        join tb4 in _dbContext.Users on tb2.UserID equals tb4.UserID
                                        where tb4.UserID == filterClass.UserID && tb3.UserID == tb4.UserID && tb.IS_ACTIVE &&
                                        (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        select new ReportInvoice
                                        {
                                            HEADER_ID = tb.HEADER_ID,
                                            INV_NO = tb.INV_NO,
                                            ODIN = tb.ODIN,
                                            ITEM_ID = tb1.ITEM_ID,
                                            ITEM_NO = tb1.ITEM_NO,
                                            INV_DATE = tb.INV_DATE,
                                            INV_TYPE = tb.INV_TYPE,
                                            CUSTOMER = tb.CUSTOMER,
                                            CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                            ORGANIZATION = tb5.Description,
                                            DIVISION = tb.DIVISION,
                                            LR_NO = tb.LR_NO,
                                            LR_DATE = tb.LR_DATE,
                                            VEHICLE_NO = tb.VEHICLE_NO,
                                            FWD_AGENT = tb.FWD_AGENT,
                                            CARRIER = tb.CARRIER,
                                            VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                            EWAYBILL_NO = tb.EWAYBILL_NO,
                                            EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                            FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                            FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                            MATERIAL_CODE = tb1.MATERIAL_CODE,
                                            MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                            QUANTITY = tb1.QUANTITY,
                                            RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                            QUANTITY_UOM = tb1.QUANTITY_UOM,
                                            PLANT = tb.PLANT,
                                            PLANT_NAME = tb.PLANT_NAME,
                                            OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                            OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                            ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                            PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                            VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                            ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                            POD_UPLOADE_STATUS = tb.STATUS,
                                            TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                            CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                            STATUS = tb.STATUS,
                                            CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                            CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                            CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                            GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                            PLANT_CODE = tb.PLANT_CODE,
                                            SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                            REMARKS = tb1.REMARKS,
                                            DISTANCE = tb.DISTANCE,
                                            ITEM_WEIGHT = tb1.ITEM_WEIGHT,
                                            DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                            TRACKING_LINK = tb.TRACKING_LINK,
                                            TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                            TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                            DELIVERY_DATE = tb.DELIVERY_DATE,
                                            DELIVERY_TIME = tb.DELIVERY_TIME
                                        }).OrderBy(x => x.INV_NO).ToListAsync();


                    var PDDList = new List<ReportInvoice>();
                    if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result.Where(t => (DateTime.Now.Date <= t.PROPOSED_DELIVERY_DATE || t.PROPOSED_DELIVERY_DATE == null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("beyond") && filterClass.LeadTime.Count == 1)
                    {
                        PDDList = result.Where(t => (DateTime.Now.Date > t.PROPOSED_DELIVERY_DATE && t.PROPOSED_DELIVERY_DATE != null) && t.STATUS.ToLower() == "open").ToList();
                    }
                    else if (filterClass.LeadTime.Contains("within") && filterClass.LeadTime.Contains("beyond"))
                    {
                        PDDList = result.Where(t => t.STATUS.ToLower() == "open").ToList();
                    }

                    var DeliveryList = new List<ReportInvoice>();
                    if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result.Where(tb => (tb.PROPOSED_DELIVERY_DATE == null || tb.VEHICLE_REPORTED_DATE == null || tb.VEHICLE_REPORTED_DATE.Value.Date <= tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("late") && filterClass.Delivery.Count == 1)
                    {
                        DeliveryList = result.Where(tb => (tb.PROPOSED_DELIVERY_DATE != null && tb.VEHICLE_REPORTED_DATE != null && tb.VEHICLE_REPORTED_DATE.Value.Date > tb.PROPOSED_DELIVERY_DATE.Value.Date) && tb.STATUS.ToLower() != "open").ToList();
                    }
                    else if (filterClass.Delivery.Contains("ontime") && filterClass.Delivery.Contains("late"))
                    {
                        DeliveryList = result.Where(t => t.STATUS.ToLower() != "open").ToList();
                    }

                    if (filterClass.Delivery.Count > 0 || filterClass.LeadTime.Count > 0)
                    {
                        result = PDDList.Concat(DeliveryList).ToList();
                    }
                    foreach (var header in result)
                    {
                        var sector = _dbContext.CustomerGroups.FirstOrDefault(t => t.CustomerGroupCode == header.CUSTOMER_GROUP_DESC);
                        if (sector != null)
                        {
                            header.SECTOR_DESCRIPTION = sector.Sector;
                        }
                    }

                    using (var workbook = new XSSFWorkbook())
                    {
                        ISheet sheet = _excelHelper.CreateNPOIworksheetReport(result, workbook);
                        using (var stream = new MemoryStream())
                        {
                            workbook.Write(stream);
                            return stream.ToArray();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> DownloadInvoiceDetailsForAutomation(FilterClass filterClass)
        {
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            CreateReportsFolder();
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

            var status = filterClass.Status;
            var today = DateTime.Now;
            var lastday = DateTime.Now.AddDays(-30);

            try
            {
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    join tb1 in _dbContext.P_INV_ITEM_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    join tb2 in _dbContext.Organizations on tb.ORGANIZATION equals tb2.OrganizationCode
                                    where tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                    select new ReportInvoice
                                    {
                                        HEADER_ID = tb.HEADER_ID,
                                        INV_NO = tb.INV_NO,
                                        ODIN = tb.ODIN,
                                        ITEM_ID = tb1.ITEM_ID,
                                        ITEM_NO = tb1.ITEM_NO,
                                        INV_DATE = tb.INV_DATE,
                                        INV_TYPE = tb.INV_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        ORGANIZATION = tb2.Description,
                                        DIVISION = tb.DIVISION,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        FWD_AGENT = tb.FWD_AGENT,
                                        CARRIER = tb.CARRIER,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        EWAYBILL_NO = tb.EWAYBILL_NO,
                                        EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                        FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                        MATERIAL_CODE = tb1.MATERIAL_CODE,
                                        MATERIAL_DESCRIPTION = tb1.MATERIAL_DESCRIPTION,
                                        QUANTITY = tb1.QUANTITY,
                                        RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                        QUANTITY_UOM = tb1.QUANTITY_UOM,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        POD_UPLOADE_STATUS = tb.STATUS,
                                        TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                        CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                        STATUS = tb.STATUS,
                                        CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                        CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                        CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                        GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                        PLANT_CODE = tb.PLANT_CODE,
                                        SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                        REMARKS = tb1.REMARKS,
                                        DISTANCE = tb.DISTANCE,
                                        ITEM_WEIGHT = tb1.ITEM_WEIGHT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME
                                    }).OrderBy(x => x.INV_NO).ToListAsync();

                using (var workbook = new XSSFWorkbook())
                {
                    ISheet sheet = _excelHelper.CreateNPOIworksheetForAutomation(result, workbook);
                    using (var stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        return stream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> DowloandPODDocument(int HeaderID, int AttachmentID)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                var att = await _dbContext.P_INV_ATTACHMENT.Where(x => x.HEADER_ID == HeaderID && x.ATTACHMENT_ID == AttachmentID).FirstOrDefaultAsync();
                string SharedFolderUserName = _networkCredentials.SharedFolderUserName;
                string SharedFolderPassword = _networkCredentials.SharedFolderPassword;
                string SharedFolderDomain = _networkCredentials.SharedFolderDomain;

                if (att != null)
                {
                    try
                    {
                        using (var impersonationHelper = new ImpersonationHelper(SharedFolderUserName, SharedFolderDomain, SharedFolderPassword))
                        {
                            var fileContent = File.ReadAllBytes(att.FILE_PATH);
                            var statuscode = HttpStatusCode.OK;
                            response = new HttpResponseMessage(statuscode);
                            response.Content = new StreamContent(new MemoryStream(fileContent));
                            response.Content.Headers.Add("x-filename", "test");
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = att.FILE_NAME
                            };
                            return response;
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        throw ex;
                    }
                    catch (IOException ex)
                    {
                        throw ex;
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            throw new Exception("Inner exception: " + ex.InnerException.Message);
                        }
                        throw ex;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CreateTempFolder()
        {
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            else
            {
                string[] files = Directory.GetFiles(TempFolder);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTime < DateTime.Now.AddMinutes(-30))
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        fi.Delete();
                    }
                }
            }
        }


        public void CreateReportsFolder()
        {
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AutomationReports");
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            else
            {
                string[] files = Directory.GetFiles(TempFolder);
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.CreationTime < DateTime.Now.AddMinutes(-30))
                    {
                        System.GC.Collect();
                        System.GC.WaitForPendingFinalizers();
                        fi.Delete();
                    }
                }
            }
        }
    }
}
