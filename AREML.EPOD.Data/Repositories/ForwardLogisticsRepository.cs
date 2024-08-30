using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Dtos.ForwardLogistics;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Data.Logging;
using AREML.EPOD.Interfaces.IRepositories;
using iTextSharp.text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Net;
using System.Net.Http.Headers;


namespace AREML.EPOD.Data.Repositories
{
    public class ForwardLogisticsRepository : IForwardLogisticsRepository
    {

        private readonly AuthContext _dbContext;
        private readonly ExcelHelper _excelHelper;
        private readonly PdfCompresser _pdfCompresser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NetworkCredentials _networkCredential;

        public ForwardLogisticsRepository(IConfiguration configuration, AuthContext authContext, ExcelHelper excel, PdfCompresser pdfCompresser, IHttpContextAccessor httpContext)
        {
            this._dbContext = authContext;
            this._excelHelper = excel;
            _pdfCompresser = pdfCompresser;
            _httpContextAccessor = httpContext;
            _networkCredential = configuration.GetSection("NetworkCredentials").Get<NetworkCredentials>();
        }

        public async Task<List<Invoice_Header_View>> GetAllInvoiceDetails(Guid UserID)
        {
            try
            {
                var userResult = (from td in _dbContext.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    select new Invoice_Header_View
                                    {
                                        HEADER_ID = td.HEADER_ID,
                                        ORGANIZATION = td.ORGANIZATION,
                                        DIVISION = td.DIVISION,
                                        PLANT = td.PLANT,
                                        PLANT_NAME = td.PLANT_NAME,
                                        INV_NO = td.INV_NO,
                                        ODIN = td.ODIN,
                                        INV_DATE = td.INV_DATE,
                                        INV_TYPE = td.INV_TYPE,
                                        CUSTOMER = td.CUSTOMER,
                                        CUSTOMER_NAME = td.CUSTOMER_NAME,
                                        CUSTOMER_GROUP = td.CUSTOMER_GROUP,
                                        VEHICLE_NO = td.VEHICLE_NO,
                                        VEHICLE_CAPACITY = td.VEHICLE_CAPACITY,
                                        POD_DATE = td.POD_DATE,
                                        EWAYBILL_NO = td.EWAYBILL_NO,
                                        EWAYBILL_DATE = td.EWAYBILL_DATE,
                                        LR_NO = td.LR_NO,
                                        LR_DATE = td.LR_DATE,
                                        FWD_AGENT = td.FWD_AGENT,
                                        CARRIER = td.CARRIER,
                                        FREIGHT_ORDER = td.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = td.FREIGHT_ORDER_DATE,
                                        OUTBOUND_DELIVERY = td.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = td.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = td.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = td.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = td.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = td.ACTUAL_DELIVERY_DATE,
                                        TRANSIT_LEAD_TIME = td.TRANSIT_LEAD_TIME,
                                        DISTANCE = td.DISTANCE,
                                        CANC_INV_STATUS = td.CANC_INV_STATUS,
                                        STATUS = td.STATUS,
                                        STATUS_DESCRIPTION = td.STATUS_DESCRIPTION,
                                        ISXMLCREATED = td.ISXMLCREATED,
                                        XMLMOVED_ON = td.XMLMOVED_ON,
                                        CREATED_BY = td.CREATED_BY,
                                        CREATED_ON = td.CREATED_ON,
                                        IS_ACTIVE = td.IS_ACTIVE,
                                        CUSTOMER_GROUP_DESC = td.CUSTOMER_GROUP_DESC,
                                        SECTOR_DESCRIPTION = td.SECTOR_DESCRIPTION,
                                        CUSTOMER_DESTINATION = td.CUSTOMER_DESTINATION,
                                        PLANT_CODE = td.PLANT_CODE,
                                        GROSS_WEIGHT = td.GROSS_WEIGHT,
                                        DRIVER_CONTACT = td.DRIVER_CONTACT,
                                        TRACKING_LINK = td.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = td.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = td.TOTAL_DISTANCE,
                                        DELIVERY_DATE = td.DELIVERY_DATE,
                                        DELIVERY_TIME = td.DELIVERY_TIME
                                    }).Where(x => (isAllOrganization || Organizations.Any(y => y == x.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == x.PLANT)) && x.IS_ACTIVE).ToListAsync();

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

        public async Task<List<P_INV_HEADER_DETAIL>> GetAllInvoiceDetailByUser(string UserCode)
        {
            try
            {
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    where td.CUSTOMER == UserCode && td.IS_ACTIVE
                                    select td).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<Invoice_Header_View>> GetOpenAndSavedInvoiceDetailByUser(string UserCode)
        {
            try
            {
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    where td.CUSTOMER == UserCode && td.IS_ACTIVE && (td.STATUS.ToLower() == "open" || td.STATUS.ToLower() == "saved")
                                    select new Invoice_Header_View
                                    {
                                        HEADER_ID = td.HEADER_ID,
                                        ORGANIZATION = td.ORGANIZATION,
                                        DIVISION = td.DIVISION,
                                        PLANT = td.PLANT,
                                        PLANT_NAME = td.PLANT_NAME,
                                        INV_NO = td.INV_NO,
                                        ODIN = td.ODIN,
                                        INV_DATE = td.INV_DATE,
                                        INV_TYPE = td.INV_TYPE,
                                        CUSTOMER = td.CUSTOMER,
                                        CUSTOMER_NAME = td.CUSTOMER_NAME,
                                        CUSTOMER_GROUP = td.CUSTOMER_GROUP,
                                        VEHICLE_NO = td.VEHICLE_NO,
                                        VEHICLE_CAPACITY = td.VEHICLE_CAPACITY,
                                        POD_DATE = td.POD_DATE,
                                        EWAYBILL_NO = td.EWAYBILL_NO,
                                        EWAYBILL_DATE = td.EWAYBILL_DATE,
                                        LR_NO = td.LR_NO,
                                        LR_DATE = td.LR_DATE,
                                        FWD_AGENT = td.FWD_AGENT,
                                        CARRIER = td.CARRIER,
                                        FREIGHT_ORDER = td.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = td.FREIGHT_ORDER_DATE,
                                        OUTBOUND_DELIVERY = td.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = td.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = td.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = td.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = td.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = td.ACTUAL_DELIVERY_DATE,
                                        TRANSIT_LEAD_TIME = td.TRANSIT_LEAD_TIME,
                                        DISTANCE = td.DISTANCE,
                                        CANC_INV_STATUS = td.CANC_INV_STATUS,
                                        STATUS = td.STATUS,
                                        STATUS_DESCRIPTION = td.STATUS_DESCRIPTION,
                                        ISXMLCREATED = td.ISXMLCREATED,
                                        XMLMOVED_ON = td.XMLMOVED_ON,
                                        CREATED_BY = td.CREATED_BY,
                                        CREATED_ON = td.CREATED_ON,
                                        IS_ACTIVE = td.IS_ACTIVE,
                                        CUSTOMER_GROUP_DESC = td.CUSTOMER_GROUP_DESC,
                                        SECTOR_DESCRIPTION = td.SECTOR_DESCRIPTION,
                                        CUSTOMER_DESTINATION = td.CUSTOMER_DESTINATION,
                                        PLANT_CODE = td.PLANT_CODE,
                                        GROSS_WEIGHT = td.GROSS_WEIGHT,
                                        DRIVER_CONTACT = td.DRIVER_CONTACT,
                                        TRACKING_LINK = td.TRACKING_LINK,
                                        TOTAL_TRAVEL_TIME = td.TOTAL_TRAVEL_TIME,
                                        TOTAL_DISTANCE = td.TOTAL_DISTANCE,
                                        DELIVERY_DATE = td.DELIVERY_DATE,
                                        DELIVERY_TIME = td.DELIVERY_TIME
                                    }).ToListAsync();
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


        public async Task<List<Invoice_Header_View>> FilterInvoiceDetailByUser(FilterClass filterClass)
        {
            try
            {
                var today = DateTime.Now;
                var lastday = DateTime.Now.AddDays(-30);
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isLRNumber = !string.IsNullOrEmpty(filterClass.LRNumber);
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == filterClass.UserCode && (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                    (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber))) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber)
                                    select new Invoice_Header_View
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
                                        CUSTOMER_GROUP = tb.CUSTOMER_GROUP,
                                        VEHICLE_NO = tb.VEHICLE_NO,
                                        VEHICLE_CAPACITY = tb.VEHICLE_CAPACITY,
                                        POD_DATE = tb.POD_DATE,
                                        EWAYBILL_NO = tb.EWAYBILL_NO,
                                        EWAYBILL_DATE = tb.EWAYBILL_DATE,
                                        LR_NO = tb.LR_NO,
                                        LR_DATE = tb.LR_DATE,
                                        FWD_AGENT = tb.FWD_AGENT,
                                        CARRIER = tb.CARRIER,
                                        FREIGHT_ORDER = tb.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = tb.FREIGHT_ORDER_DATE,
                                        OUTBOUND_DELIVERY = tb.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = tb.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = tb.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = tb.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = tb.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = tb.ACTUAL_DELIVERY_DATE,
                                        TRANSIT_LEAD_TIME = tb.TRANSIT_LEAD_TIME,
                                        DISTANCE = tb.DISTANCE,
                                        CANC_INV_STATUS = tb.CANC_INV_STATUS,
                                        STATUS = tb.STATUS,
                                        STATUS_DESCRIPTION = tb.STATUS_DESCRIPTION,
                                        ISXMLCREATED = tb.ISXMLCREATED,
                                        XMLMOVED_ON = tb.XMLMOVED_ON,
                                        CREATED_BY = tb.CREATED_BY,
                                        CREATED_ON = tb.CREATED_ON,
                                        IS_ACTIVE = tb.IS_ACTIVE,
                                        CUSTOMER_GROUP_DESC = tb.CUSTOMER_GROUP_DESC,
                                        SECTOR_DESCRIPTION = tb.SECTOR_DESCRIPTION,
                                        CUSTOMER_DESTINATION = tb.CUSTOMER_DESTINATION,
                                        PLANT_CODE = tb.PLANT_CODE,
                                        GROSS_WEIGHT = tb.GROSS_WEIGHT,
                                        TRACKING_LINK = tb.TRACKING_LINK,
                                        DRIVER_CONTACT = tb.DRIVER_CONTACT,
                                        TOTAL_DISTANCE = tb.TOTAL_DISTANCE,
                                        TOTAL_TRAVEL_TIME = tb.TOTAL_TRAVEL_TIME,
                                        DELIVERY_DATE = tb.DELIVERY_DATE,
                                        DELIVERY_TIME = tb.DELIVERY_TIME,
                                        INVOICE_QUANTITY = (from items in _dbContext.P_INV_ITEM_DETAIL where items.HEADER_ID == tb.HEADER_ID select items.QUANTITY).Sum()
                                    }).ToListAsync();
                var PDDList = new List<Invoice_Header_View>();
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

                var DeliveryList = new List<Invoice_Header_View>();
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
        public async Task<byte[]> DownloadInvoiceDetailByUser(FilterClass filterClass)
        {
            try
            {
                //var response = new HttpResponseMessage(HttpStatusCode.OK);
                CreateTempFolder();
                string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isLRNumber = !string.IsNullOrEmpty(filterClass.LRNumber);
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.CUSTOMER == filterClass.UserCode && (!isStatus || filterClass.Status.Any(x => x == tb.STATUS)) && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date)) &&
                                    (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber))) && (!isLRNumber || tb.LR_NO == filterClass.LRNumber)
                                    select tb).ToListAsync();
                var PDDList = new List<P_INV_HEADER_DETAIL>();
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

                var DeliveryList = new List<P_INV_HEADER_DETAIL>();
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
                    var Result = _dbContext.CustomerGroups.FirstOrDefault(t => t.CustomerGroupCode == header.CUSTOMER_GROUP_DESC);
                    if (Result != null)
                    {
                        header.SECTOR_DESCRIPTION = Result.Sector;
                    }
                }

                using (var workbook = new XSSFWorkbook())
                {
                    ISheet sheet = _excelHelper.CreateNPOIworksheet(result, false, workbook);
                    using (var stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        return stream.ToArray();
                    }
                }

                //IWorkbook workbook = new XSSFWorkbook();
                //ISheet sheet = _excelHelper.CreateNPOIworksheet(result, false, workbook);
                //DateTime dt1 = DateTime.Today;
                //string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                //var FileNm = $"Invoice details_{dtstr1}.xlsx";
                //var FilePath = Path.Combine(TempFolder, FileNm);
                //if (System.IO.File.Exists(FilePath))
                //{
                //    System.GC.Collect();
                //    System.GC.WaitForPendingFinalizers();
                //    System.IO.File.Delete(FilePath);
                //}
                //FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                //workbook.Write(stream);
                //byte[] fileByteArray = System.IO.File.ReadAllBytes(FilePath);
                //var statuscode = HttpStatusCode.OK;
                //response.Content = new ByteArrayContent(fileByteArray);
                //response.Content.Headers.Add("x-filename", FileNm);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //ContentDispositionHeaderValue contentDisposition = null;

                //if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                //{
                //    response.Content.Headers.ContentDisposition = contentDisposition;
                //}

                //return response;
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


        public async Task<List<Invoice_Header_View>> GetConfirmedInvoiceDetails(Guid UserID)
        {
            try
            {
                var StartDate = DateTime.Now.AddDays(-30);
                var EndDate = DateTime.Now;
                var userResult = (from td in _dbContext.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var Organizations = _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToList();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToList();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    where (isAllOrganization || Organizations.Any(y => y == td.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == td.PLANT)) && td.STATUS.ToLower() == "confirmed" &&
                                    td.INV_DATE.Value.Date >= StartDate.Date && td.IS_ACTIVE &&
                                    td.INV_DATE.Value.Date <= EndDate.Date
                                    select new Invoice_Header_View
                                    {
                                        HEADER_ID = td.HEADER_ID,
                                        ORGANIZATION = td.ORGANIZATION,
                                        DIVISION = td.DIVISION,
                                        PLANT = td.PLANT,
                                        PLANT_NAME = td.PLANT_NAME,
                                        INV_NO = td.INV_NO,
                                        ODIN = td.ODIN,
                                        INV_DATE = td.INV_DATE,
                                        INV_TYPE = td.INV_TYPE,
                                        CUSTOMER = td.CUSTOMER,
                                        CUSTOMER_NAME = td.CUSTOMER_NAME,
                                        CUSTOMER_GROUP = td.CUSTOMER_GROUP,
                                        VEHICLE_NO = td.VEHICLE_NO,
                                        VEHICLE_CAPACITY = td.VEHICLE_CAPACITY,
                                        POD_DATE = td.POD_DATE,
                                        EWAYBILL_NO = td.EWAYBILL_NO,
                                        EWAYBILL_DATE = td.EWAYBILL_DATE,
                                        LR_NO = td.LR_NO,
                                        LR_DATE = td.LR_DATE,
                                        FWD_AGENT = td.FWD_AGENT,
                                        CARRIER = td.CARRIER,
                                        FREIGHT_ORDER = td.FREIGHT_ORDER,
                                        FREIGHT_ORDER_DATE = td.FREIGHT_ORDER_DATE,
                                        OUTBOUND_DELIVERY = td.OUTBOUND_DELIVERY,
                                        OUTBOUND_DELIVERY_DATE = td.OUTBOUND_DELIVERY_DATE,
                                        ACTUAL_DISPATCH_DATE = td.ACTUAL_DISPATCH_DATE,
                                        PROPOSED_DELIVERY_DATE = td.PROPOSED_DELIVERY_DATE,
                                        VEHICLE_REPORTED_DATE = td.VEHICLE_REPORTED_DATE,
                                        ACTUAL_DELIVERY_DATE = td.ACTUAL_DELIVERY_DATE,
                                        TRANSIT_LEAD_TIME = td.TRANSIT_LEAD_TIME,
                                        DISTANCE = td.DISTANCE,
                                        CANC_INV_STATUS = td.CANC_INV_STATUS,
                                        STATUS = td.STATUS,
                                        STATUS_DESCRIPTION = td.STATUS_DESCRIPTION,
                                        ISXMLCREATED = td.ISXMLCREATED,
                                        XMLMOVED_ON = td.XMLMOVED_ON,
                                        CREATED_BY = td.CREATED_BY,
                                        CREATED_ON = td.CREATED_ON,
                                        IS_ACTIVE = td.IS_ACTIVE,
                                        CUSTOMER_GROUP_DESC = td.CUSTOMER_GROUP_DESC,
                                        SECTOR_DESCRIPTION = td.SECTOR_DESCRIPTION,
                                        CUSTOMER_DESTINATION = td.CUSTOMER_DESTINATION,
                                        PLANT_CODE = td.PLANT_CODE,
                                        GROSS_WEIGHT = td.GROSS_WEIGHT,
                                        TRACKING_LINK = td.TRACKING_LINK,
                                        DRIVER_CONTACT = td.DRIVER_CONTACT,
                                        TOTAL_DISTANCE = td.TOTAL_DISTANCE,
                                        TOTAL_TRAVEL_TIME = td.TOTAL_TRAVEL_TIME,
                                        DELIVERY_DATE = td.DELIVERY_DATE,
                                        DELIVERY_TIME = td.DELIVERY_TIME,
                                    }).ToListAsync();
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

        public async Task<bool> ApproveSelectedInvoices(ApproverDetails approverDetails)
        {
            try
            {
                if (approverDetails.HEADERIDs.Count > 0)
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in approverDetails.HEADERIDs on tb.HEADER_ID equals tb1
                                        where tb.IS_ACTIVE
                                        select tb).ToListAsync();
                    foreach (P_INV_HEADER_DETAIL header in result)
                    {
                        header.STATUS = "Approved";
                        List<P_INV_ITEM_DETAIL> items = _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == header.HEADER_ID).ToList();
                        items.ForEach(x =>
                        {
                            x.STATUS = "Approved";
                        });
                    }
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByHeaderID(int HeaderID)
        {
            try
            {
                var result = await (from tb in _dbContext.P_INV_ITEM_DETAIL
                                    join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    where tb.HEADER_ID == HeaderID && tb.IS_ACTIVE
                                    orderby tb.ITEM_NO
                                    select tb).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByID(Guid UserID, int ID)
        {
            try
            {
                var userResult = (from td in _dbContext.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");
                var result = await (from tb in _dbContext.P_INV_ITEM_DETAIL
                                    join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    where tb.HEADER_ID == ID && tb.IS_ACTIVE &&
                                    (isAllOrganization || Organizations.Any(y => y == tb1.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == tb1.PLANT))
                                    orderby tb.ITEM_NO
                                    select tb).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<P_INV_ITEM_DETAIL>> GetInvoiceItemDetailsByUserAndID(string UserCode, int ID)
        {
            try
            {
                var result = await (from tb in _dbContext.P_INV_ITEM_DETAIL
                                    join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.HEADER_ID equals tb1.HEADER_ID
                                    where tb.HEADER_ID == ID && tb1.CUSTOMER == UserCode && tb.IS_ACTIVE
                                    orderby tb.ITEM_NO
                                    select tb).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<P_INV_HEADER_DETAIL> UpdateInvoiceItems(InvoiceUpdation invoiceUpdation)
        {
            if (invoiceUpdation.VEHICLE_REPORTED_DATE != null && !invoiceUpdation.VEHICLE_REPORTED_DATE.ToString().Contains("1970"))
            {
                try
                {
                    P_INV_HEADER_DETAIL head = null;
                    if (invoiceUpdation.InvoiceItems.Count > 0)
                    {
                        var HeaderID = invoiceUpdation.InvoiceItems[0].HEADER_ID;
                        head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(t => t.HEADER_ID == HeaderID);
                        SalesReturnProps salesReturnProps = new SalesReturnProps();
                        bool IsPartiallyConfirmed = false;
                        if (head.STATUS.ToLower() != "confirmed")
                        {
                            foreach (P_INV_ITEM_DETAIL item in invoiceUpdation.InvoiceItems)
                            {
                                P_INV_ITEM_DETAIL ite = _dbContext.P_INV_ITEM_DETAIL.FirstOrDefault(x => x.ITEM_ID == item.ITEM_ID && x.HEADER_ID == item.HEADER_ID);
                                if (ite != null)
                                {
                                    ite.RECEIVED_QUANTITY = item.RECEIVED_QUANTITY;
                                    ite.REASON = item.REASON;
                                    ite.REMARKS = item.REMARKS;
                                    if (item.STATUS.ToLower() == "saved")
                                    {
                                        if (ite.STATUS.ToLower() != "confirmed")
                                        {
                                            ite.STATUS = item.STATUS;
                                        }
                                    }
                                    else if (item.STATUS.ToLower() != "confirmed")
                                    {
                                        ite.STATUS = item.STATUS;
                                    }
                                    else if (item.STATUS.ToLower() == "confirmed")
                                    {
                                        ite.STATUS = ite.RECEIVED_QUANTITY != item.QUANTITY ? "PartiallyConfirmed" : "Confirmed";
                                        if (!IsPartiallyConfirmed)
                                        {
                                            IsPartiallyConfirmed = ite.RECEIVED_QUANTITY != item.QUANTITY;
                                        }
                                    }
                                }
                            }
                            await _dbContext.SaveChangesAsync();
                            head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == HeaderID && x.IS_ACTIVE);
                            if (head != null)
                            {
                                if (head.INV_DATE.Value.Date <= invoiceUpdation.VEHICLE_REPORTED_DATE.Date)
                                {
                                    if (invoiceUpdation.InvoiceItems[0].STATUS.ToLower() == "saved")
                                    {
                                        if (head.STATUS.ToLower() != "confirmed")
                                        {
                                            head.STATUS = invoiceUpdation.InvoiceItems[0].STATUS;
                                        }
                                    }
                                    else if (invoiceUpdation.InvoiceItems[0].STATUS.ToLower() != "confirmed")
                                    {
                                        head.STATUS = invoiceUpdation.InvoiceItems[0].STATUS;
                                    }
                                    else if (invoiceUpdation.InvoiceItems[0].STATUS.ToLower() == "confirmed")
                                    {
                                        head.STATUS = IsPartiallyConfirmed ? "PartiallyConfirmed" : "Confirmed";
                                        head.POD_DATE = DateTime.Now;
                                        head.ACTUAL_DELIVERY_DATE = head.STATUS == "Confirmed" ? DateTime.Now : (DateTime?)null;
                                    }
                                    head.VEHICLE_REPORTED_DATE = invoiceUpdation.VEHICLE_REPORTED_DATE;
                                    if (head.ISXMLCREATED)
                                    {
                                        head.ISXMLCREATED = false;
                                    }

                                }
                                else
                                {
                                    throw new Exception("Vehicle reported date can not be earlier than Invoice date");
                                }
                            }
                        }
                    }
                    await _dbContext.SaveChangesAsync();
                    return head != null ? head : new P_INV_HEADER_DETAIL();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Vehicle Reported Date field can not be empty");
            }
        }


        public async Task<bool> ConfirmInvoiceItems(InvoiceUpdation1 invoiceUpdation)
        {

            if (invoiceUpdation.VEHICLE_REPORTED_DATE != null && !invoiceUpdation.VEHICLE_REPORTED_DATE.ToString().Contains("1970"))
            {
                try
                {
                    List<P_INV_ITEM_DETAIL> items = await _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == invoiceUpdation.HEADER_ID).ToListAsync();

                    foreach (P_INV_ITEM_DETAIL ite in items)
                    {
                        if (ite != null)
                        {
                            ite.RECEIVED_QUANTITY = ite.QUANTITY;
                            ite.STATUS = "Confirmed";
                        }
                    }
                    P_INV_HEADER_DETAIL head = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(x => x.HEADER_ID == invoiceUpdation.HEADER_ID && x.IS_ACTIVE);
                    if (head != null)
                    {
                        if (head.INV_DATE.Value.Date <= invoiceUpdation.VEHICLE_REPORTED_DATE.Date)
                        {
                            head.STATUS = "Confirmed";
                            head.POD_DATE = DateTime.Now;
                            head.ACTUAL_DELIVERY_DATE = DateTime.Now;
                            head.VEHICLE_REPORTED_DATE = invoiceUpdation.VEHICLE_REPORTED_DATE;
                            if (head.ISXMLCREATED)
                            {
                                head.ISXMLCREATED = false;
                            }
                        }
                        else
                        {
                            throw new Exception("Vehicle reported date can not be earlier than Invoice date");
                        }
                    }
                    else
                    {
                        throw new Exception("Header ID  is null in DB");
                    }
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Vehicle Reported Date field can not be empty");
            }
        }

        public async Task<AttachmentStatus> GetAttachmentID(int headerId)
        {
            var doc = await _dbContext.P_INV_ATTACHMENT.FirstOrDefaultAsync(t => t.HEADER_ID == headerId);
            if (doc == null)
            {
                return new AttachmentStatus { AttachmentId = 0 };
            }
            else
            {
                return new AttachmentStatus { AttachmentId = doc.ATTACHMENT_ID };
            }
        }

        public async Task<bool> CreateUserActionHistory(UserActionHistory log)
        {
            try
            {
                this._dbContext.UserActionHistories.Add(log);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<UserActionHistoryView>> GetUserActionHistories(ActionHistoryFilter filter)
        {
            try
            {
                bool isFromDate = filter.StartDate.HasValue;
                bool isEndDate = filter.EndDate.HasValue;
                bool isUserName = !string.IsNullOrEmpty(filter.UserName);
                bool isInvoiceNumber = !string.IsNullOrEmpty(filter.InvoiceNumber);
                var logs = await (from tb in _dbContext.UserActionHistories
                                  join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.TransID equals tb1.HEADER_ID
                                  where
                                ((!isFromDate || tb1.INV_DATE.Value.Date >= filter.StartDate.Value.Date) &&
                                (!isEndDate || tb1.INV_DATE.Value.Date <= filter.EndDate.Value.Date) &&
                                (!isUserName || tb.UserName.ToLower().Contains(filter.UserName.ToLower())) &&
                                (!isInvoiceNumber || (tb1.INV_NO == filter.InvoiceNumber || tb1.ODIN == filter.InvoiceNumber)))
                                  select new UserActionHistoryView
                                  {
                                      InvoiceNumber = isInvoiceNumber ? tb1.ODIN == filter.InvoiceNumber ? tb1.ODIN : tb1.INV_NO : tb1.ODIN,
                                      UserName = tb.UserName,
                                      IpAddress = tb.IpAddress,
                                      Location = tb.Location,
                                      Action = tb.Action,
                                      ChangesDetected = tb.ChangesDetected,
                                      DateTime = tb.DateTime
                                  }).ToListAsync();
                return logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<HttpResponseMessage> DownloadActionHistoryLog(ActionHistoryFilter filter)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                CreateTempFolder();
                string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                bool isFromDate = filter.StartDate.HasValue;
                bool isEndDate = filter.EndDate.HasValue;
                bool isUserName = !string.IsNullOrEmpty(filter.UserName);
                bool isInvoiceNumber = !string.IsNullOrEmpty(filter.InvoiceNumber);
                var logs = await (from tb in _dbContext.UserActionHistories
                                  join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.TransID equals tb1.HEADER_ID
                                  where
                                ((!isFromDate || tb.DateTime.Date >= filter.StartDate.Value.Date) &&
                      (!isEndDate || tb.DateTime.Date <= filter.EndDate.Value.Date) &&
                      (!isUserName || tb.UserName.ToLower().Contains(filter.UserName)) &&
                      (!isInvoiceNumber || (tb1.INV_NO.ToLower() == filter.InvoiceNumber.ToLower() || tb1.ODIN.ToLower() == filter.InvoiceNumber.ToLower())))
                                  select new UserActionHistoryView
                                  {
                                      InvoiceNumber = tb1.ODIN,
                                      UserName = tb.UserName,
                                      IpAddress = tb.IpAddress,
                                      Location = tb.Location,
                                      Action = tb.Action,
                                      ChangesDetected = tb.ChangesDetected,
                                      DateTime = tb.DateTime
                                  }).ToListAsync();

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = _excelHelper.CreateActionHistoryWorksheet(logs, true, workbook);
                DateTime dt1 = DateTime.Today;
                string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                var FileNm = $"ActionHistories_{dtstr1}.xlsx";
                var FilePath = Path.Combine(TempFolder, FileNm);
                if (System.IO.File.Exists(FilePath))
                {
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(FilePath);
                }
                FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                workbook.Write(stream);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(FilePath);
                var statuscode = HttpStatusCode.OK;
                response.Content = new StreamContent(new MemoryStream(fileByteArray));
                response.Content.Headers.Add("x-filename", FileNm);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                ContentDispositionHeaderValue contentDisposition = null;

                if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                {
                    response.Content.Headers.ContentDisposition = contentDisposition;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddInvoiceAttachment()
        {
            var HeaderID = 0;
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                HeaderID = int.Parse(request.Form["HeaderID"].ToString());
                var CreatedBy = request.Form["CreatedBy"].ToString();

                if (request.Form.Files.Count > 0)
                {
                    for (int i = 0; i < request.Form.Files.Count; i++)
                    {
                        var file = request.Form.Files[i];
                        var FileName = file.FileName;
                        using (var memoryStream = new MemoryStream())
                        {
                            await file.CopyToAsync(memoryStream);
                            byte[] fileBytes = memoryStream.ToArray();
                            if (fileBytes.Length > 0)
                            {
                                ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPDF(FileName, fileBytes);
                                P_INV_HEADER_DETAIL header = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(t => t.HEADER_ID == HeaderID);

                                var plGrps = await _dbContext.PlantGroupPlantMaps.Where(x => x.PlantGroupId == 4).Select(p => p.PlantCode).ToListAsync();
                                if (plGrps != null && plGrps.Count > 0 && plGrps.Contains(header.PLANT_CODE))
                                {
                                    var previousAttachment = await _dbContext.P_INV_ATTACHMENT.FirstOrDefaultAsync(x => x.HEADER_ID == HeaderID);

                                    if (previousAttachment == null)
                                    {
                                        P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                        attachment.HEADER_ID = HeaderID;
                                        attachment.ATTACHMENT_NAME = convertedAttachment.Filename;
                                        attachment.DOCUMENT_TYPE = "application/pdf";
                                        attachment.ATTACHMENT_FILE = convertedAttachment.PDFcontent;
                                        attachment.CREATED_BY = CreatedBy;
                                        attachment.CREATED_ON = DateTime.Now;
                                        attachment.IS_ACTIVE = true;
                                        _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                        await _dbContext.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        var docHistory = new DocumentHistory();
                                        docHistory.HeaderId = previousAttachment.HEADER_ID;
                                        docHistory.FileName = previousAttachment.ATTACHMENT_NAME;
                                        docHistory.FileContent = previousAttachment.ATTACHMENT_FILE;
                                        docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                        docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                        docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                        _dbContext.DocumentHistories.Add(docHistory);
                                        previousAttachment.ATTACHMENT_NAME = convertedAttachment.Filename;
                                        previousAttachment.ATTACHMENT_FILE = convertedAttachment.PDFcontent;
                                        previousAttachment.CREATED_ON = DateTime.Now;
                                        await _dbContext.SaveChangesAsync();
                                    }
                                }
                                else
                                {
                                    var headerList = await _dbContext.P_INV_HEADER_DETAIL.Where(t => t.LR_NO == header.LR_NO && t.CUSTOMER == header.CUSTOMER && t.LR_DATE == header.LR_DATE).Select(t => t.HEADER_ID).ToListAsync();
                                    foreach (var headerId in headerList)
                                    {
                                        var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == headerId);

                                        if (previousAttachment == null)
                                        {
                                            P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                            attachment.HEADER_ID = headerId;
                                            attachment.ATTACHMENT_NAME = convertedAttachment.Filename;
                                            attachment.DOCUMENT_TYPE = "application/pdf";
                                            attachment.ATTACHMENT_FILE = convertedAttachment.PDFcontent;
                                            attachment.CREATED_BY = CreatedBy;
                                            attachment.CREATED_ON = DateTime.Now;
                                            attachment.IS_ACTIVE = true;
                                            _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            var docHistory = new DocumentHistory();
                                            docHistory.HeaderId = previousAttachment.HEADER_ID;
                                            docHistory.FileName = previousAttachment.ATTACHMENT_NAME;
                                            docHistory.FileContent = previousAttachment.ATTACHMENT_FILE;
                                            docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                            docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                            docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                            _dbContext.DocumentHistories.Add(docHistory);
                                            previousAttachment.ATTACHMENT_NAME = convertedAttachment.Filename;
                                            previousAttachment.ATTACHMENT_FILE = convertedAttachment.PDFcontent;
                                            previousAttachment.CREATED_ON = DateTime.Now;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }

                            }

                        }
                    }
                    await UpdateInvoiceStatus(HeaderID, "Confirmed");
                }
                else
                {
                    throw new Exception("No files found");
                }
            }
            catch (Exception ex)
            {
                await UpdateInvoiceStatus(HeaderID, "Saved");
                throw ex;

            }
            return true;
        }


        private async Task UpdateInvoiceStatus(int HeaderID, string Status)
        {
            try
            {
                bool IsPartiallyConfirmed = false;
                List<P_INV_ITEM_DETAIL> InvoiceItems = _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == HeaderID).ToList();
                foreach (P_INV_ITEM_DETAIL item in InvoiceItems)
                {
                    if (Status.ToLower() == "saved")
                    {
                        item.STATUS = Status;
                    }
                    else if (Status.ToLower() == "confirmed")
                    {
                        item.STATUS = item.RECEIVED_QUANTITY != item.QUANTITY ? "PartiallyConfirmed" : "Confirmed";
                        if (!IsPartiallyConfirmed)
                        {
                            IsPartiallyConfirmed = item.RECEIVED_QUANTITY != item.QUANTITY;
                        }
                    }
                    else
                    {
                        item.STATUS = Status;
                    }
                }
                P_INV_HEADER_DETAIL head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == HeaderID && x.IS_ACTIVE);
                if (head != null)
                {
                    if (Status.ToLower() == "saved")
                    {
                        head.STATUS = Status;
                    }
                    else if (Status.ToLower() == "confirmed")
                    {
                        head.STATUS = IsPartiallyConfirmed ? "PartiallyConfirmed" : "Confirmed";
                        head.ACTUAL_DELIVERY_DATE = head.STATUS == "Confirmed" ? DateTime.Now : (DateTime?)null;
                    }
                    else
                    {
                        head.STATUS = Status;
                    }

                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetAllSavedInvoicesByUserID(Guid UserID)
        {
            try
            {
                var Organizations = await _dbContext.UserOrganizationMaps.Where(x => x.UserID == UserID).Select(y => y.OrganizationCode).ToListAsync();
                bool isAllOrganization = Organizations.Any(x => x.ToLower() == "all");
                var Plants = await _dbContext.UserPlantMaps.Where(x => x.UserID == UserID).Select(y => y.PlantCode).ToListAsync();
                bool isAllPlant = Plants.Any(x => x.ToLower() == "all");

                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.STATUS.ToLower() == "saved" && tb.IS_ACTIVE &&
                                    (isAllOrganization || Organizations.Any(y => y == tb.ORGANIZATION)) && (isAllPlant || Plants.Any(y => y == tb.PLANT))
                                    select tb).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetAllSavedInvoices()
        {
            try
            {
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    where td.STATUS.ToLower() == "saved" && td.IS_ACTIVE
                                    select td).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<InvoiceHeaderDetails>> FilterSavedInvoicesByUserID(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
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
                                        join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "saved" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
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
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS.ToLower() == "saved" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<byte[]> DownloadSavedInvoicesByUserID(FilterClass filterClass)
        {
            try
            {
                //var response = new HttpResponseMessage(HttpStatusCode.OK);
                CreateTempFolder();
                string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
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
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        join tb4 in cgs on tb.CUSTOMER_GROUP_DESC equals tb4.CustomerGroupCode
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS == "Saved" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        orderby tb.HEADER_ID
                                        select tb).ToListAsync();
                    foreach (var header in result)
                    {
                        var sector = await _dbContext.CustomerGroups.FirstOrDefaultAsync(t => t.CustomerGroupCode == header.CUSTOMER_GROUP_DESC);
                        if (sector != null)
                        {
                            header.SECTOR_DESCRIPTION = sector.Sector;
                        }
                    }
                    using (var workbook = new XSSFWorkbook())
                    {
                        ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                        using (var stream = new MemoryStream())
                        {
                            workbook.Write(stream);
                            return stream.ToArray();
                        }
                    }

                    //IWorkbook workbook = new XSSFWorkbook();
                    //ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                    //DateTime dt1 = DateTime.Today;
                    //string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                    //var FileNm = $"Saved Invoice details_{dtstr1}.xlsx";
                    //var FilePath = Path.Combine(TempFolder, FileNm);
                    //if (System.IO.File.Exists(FilePath))
                    //{
                    //    System.GC.Collect();
                    //    System.GC.WaitForPendingFinalizers();
                    //    System.IO.File.Delete(FilePath);
                    //}
                    //FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                    //workbook.Write(stream);
                    //byte[] fileByteArray = System.IO.File.ReadAllBytes(FilePath);
                    //var statuscode = HttpStatusCode.OK;
                    //response.Content = new ByteArrayContent(fileByteArray);
                    //response.Content.Headers.Add("x-filename", FileNm);
                    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //ContentDispositionHeaderValue contentDisposition = null;

                    //if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                    //{
                    //    response.Content.Headers.ContentDisposition = contentDisposition;
                    //}

                    //return response;
                }
                else
                {
                    var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                        join tb1 in _dbContext.UserOrganizationMaps on tb.ORGANIZATION equals tb1.OrganizationCode
                                        join tb2 in _dbContext.UserPlantMaps on tb.PLANT equals tb2.PlantCode
                                        join tb3 in _dbContext.Users on tb1.UserID equals tb3.UserID
                                        where tb3.UserID == filterClass.UserID && tb2.UserID == tb3.UserID && tb.IS_ACTIVE &&
                                        tb.STATUS == "Saved" &&
                                        (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                        (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                        && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                        && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                        && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                        && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                        && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                        && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                        orderby tb.HEADER_ID
                                        select tb).ToListAsync();
                    foreach (var header in result)
                    {
                        var sector = await _dbContext.CustomerGroups.FirstOrDefaultAsync(t => t.CustomerGroupCode == header.CUSTOMER_GROUP_DESC);
                        if (sector != null)
                        {
                            header.SECTOR_DESCRIPTION = sector.Sector;
                        }
                    }

                    using (var workbook = new XSSFWorkbook())
                    {
                        ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                        using (var stream = new MemoryStream())
                        {
                            workbook.Write(stream);
                            return stream.ToArray();
                        }
                    }


                    //IWorkbook workbook = new XSSFWorkbook();
                    //ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                    //DateTime dt1 = DateTime.Today;
                    //string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                    //var FileNm = $"Saved Invoice details_{dtstr1}.xlsx";
                    //var FilePath = Path.Combine(TempFolder, FileNm);
                    //if (System.IO.File.Exists(FilePath))
                    //{
                    //    System.GC.Collect();
                    //    System.GC.WaitForPendingFinalizers();
                    //    System.IO.File.Delete(FilePath);
                    //}
                    //FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                    //workbook.Write(stream);
                    //byte[] fileByteArray = System.IO.File.ReadAllBytes(FilePath);
                    //var statuscode = HttpStatusCode.OK;
                    //response.Content = new ByteArrayContent(fileByteArray);
                    //response.Content.Headers.Add("x-filename", FileNm);
                    //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //ContentDispositionHeaderValue contentDisposition = null;

                    //if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                    //{
                    //    response.Content.Headers.ContentDisposition = contentDisposition;
                    //}
                    //return response;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetAllPartiallyConfirmedInvoices()
        {
            try
            {
                var result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                    where td.STATUS.ToLower() == "partiallyconfirmed" && td.IS_ACTIVE
                                    select td).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoices(int CurrentPage, int Records, DateTime? StartDate = null, DateTime? EndDate = null, string InvoiceNumber = null, string Organization = null, string Division = null, string Plant = null, string CustomerName = null)
        {
            try
            {
                var SkipValue = (CurrentPage - 1) * Records;
                var TakeValue = Records;
                bool isFromDate = StartDate.HasValue;
                bool isEndDate = EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(InvoiceNumber);
                bool isPlant = !string.IsNullOrEmpty(Plant);
                bool isOrganization = !string.IsNullOrEmpty(Organization);
                bool isDivision = !string.IsNullOrEmpty(Division);
                bool isAllDivision = isDivision && Division.ToLower() == "all";
                bool isCustomerName = !string.IsNullOrEmpty(CustomerName);
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                                    where tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= EndDate.Value.Date))
                                    && (!isInvoiceNumber || tb.ODIN == InvoiceNumber)
                                    && (!isOrganization || tb.ORGANIZATION == Organization) && (!isDivision || isAllDivision || tb.DIVISION == Division) && (!isPlant || tb.PLANT == Plant)
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(CustomerName.ToLower()))
                                    orderby tb.HEADER_ID
                                    select tb).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

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
                                    where tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                    && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                    && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                    && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                    && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                    && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                    orderby tb.HEADER_ID
                                    select tb).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> FilterPartiallyConfirmedInvoicesForAdmin(FilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerGroup = filterClass.CustomerGroup != null && filterClass.CustomerGroup.Count > 0;

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
                                    where tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                    && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                    && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                    && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                    && (!isPlantGroup || plants.Any(x => x == tb.PLANT)) && (!isCustomerGroup || filterClass.CustomerGroup.Any(k => k == tb.CUSTOMER_GROUP_DESC))
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                    orderby tb.HEADER_ID
                                    select tb).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<byte[]> DownloadPartiallyConfirmedInvoices(FilterClass filterClass)
        {
            try
            {
                //var response = new HttpResponseMessage(HttpStatusCode.OK);
                CreateTempFolder();
                string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isInvoiceNumber = !string.IsNullOrEmpty(filterClass.InvoiceNumber);
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isOrganization = filterClass.Organization != null && filterClass.Organization.Count > 0;
                bool isDivision = filterClass.Division != null && filterClass.Division.Count > 0;
                bool isCustomerName = !string.IsNullOrEmpty(filterClass.CustomerName);

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
                                    where tb.STATUS.ToLower() == "partiallyconfirmed" && tb.IS_ACTIVE &&
                                    (!isFromDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date >= filterClass.StartDate.Value.Date)) &&
                                    (!isEndDate || (tb.INV_DATE.HasValue && tb.INV_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                    && (!isInvoiceNumber || (tb.ODIN.Contains(filterClass.InvoiceNumber) || tb.INV_NO.Contains(filterClass.InvoiceNumber)))
                                    && (!isOrganization || filterClass.Organization.Any(x => x == tb.ORGANIZATION)) && (!isDivision || filterClass.Division.Any(x => x == tb.DIVISION))
                                    && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                    && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                    && (!isCustomerName || tb.CUSTOMER_NAME.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                    orderby tb.HEADER_ID
                                    select tb).ToListAsync();

                using (var workbook = new XSSFWorkbook())
                {
                    ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                    using (var stream = new MemoryStream())
                    {
                        workbook.Write(stream);
                        return stream.ToArray();
                    }
                }

                //IWorkbook workbook = new XSSFWorkbook();
                //ISheet sheet = _excelHelper.CreateNPOIworksheet(result, true, workbook);
                //DateTime dt1 = DateTime.Today;
                //string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                //var FileNm = $"Partially Confirmed Invoice_{dtstr1}.xlsx";
                //var FilePath = Path.Combine(TempFolder, FileNm);
                //if (System.IO.File.Exists(FilePath))
                //{
                //    System.GC.Collect();
                //    System.GC.WaitForPendingFinalizers();
                //    System.IO.File.Delete(FilePath);
                //}
                //FileStream stream = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                //workbook.Write(stream);
                //byte[] fileByteArray = System.IO.File.ReadAllBytes(FilePath);
                //var statuscode = HttpStatusCode.OK;
                //response.Content = new ByteArrayContent(fileByteArray);
                //response.Content.Headers.Add("x-filename", FileNm);
                //response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //ContentDispositionHeaderValue contentDisposition = null;

                //if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                //{
                //    response.Content.Headers.ContentDisposition = contentDisposition;
                //}

                //return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> UpdatePartiallyConfirmedInvoiceStatus(int HeaderID, string Status)
        {
            try
            {
                P_INV_HEADER_DETAIL head = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(x => x.HEADER_ID == HeaderID && x.IS_ACTIVE);
                if (head != null)
                {
                    if (head.STATUS.ToLower() == "partiallyconfirmed")
                    {
                        head.STATUS = Status;
                    }
                    else
                    {
                        throw new Exception($"Given invoice status in {head.STATUS}");
                    }
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateVehicleUnloadedDateByLR(LRWithVehicleUnloaded request)
        {
            try
            {
                var headers = await _dbContext.P_INV_HEADER_DETAIL.Where(t => t.LR_NO == request.LRNumber && t.CUSTOMER == request.Customer && t.LR_DATE.Value.Date == request.LRDate.Date).ToListAsync();
                foreach (var header in headers)
                {
                    if (!header.VEHICLE_REPORTED_DATE.HasValue)
                    {
                        header.VEHICLE_REPORTED_DATE = request.VehicleUnloadedDate;
                    }
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateLRDateByInvoiceNo(LRDateUpdate req)
        {
            try
            {
                var header = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(t => t.ODIN == req.InvoiceNo || t.INV_NO == req.InvoiceNo);
                if (header == null)
                {
                    throw new Exception("Invoice not found");
                }
                else
                {
                    header.LR_DATE = req.LRDate;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateLRNumberByInvoiceNo(LRNumberUpdate req)
        {
            try
            {
                var header = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(t => t.ODIN == req.InvoiceNo || t.INV_NO == req.InvoiceNo);
                if (header == null)
                {
                    throw new Exception("Invoice not found");
                }
                else
                {
                    header.LR_NO = req.LRNumber;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SaveScrollNotification(ScrollNotification notification)
        {
            try
            {
                var scrollNotification = await _dbContext.ScrollNotifications.FirstOrDefaultAsync(t => t.Id == notification.Id && t.Code == notification.Code);
                if (scrollNotification != null)
                {
                    scrollNotification.Message = notification.Message;
                    scrollNotification.IsActive = notification.IsActive;
                    scrollNotification.Code = notification.Code;
                }
                else
                {
                    _dbContext.ScrollNotifications.Add(notification);
                }
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ScrollNotification>> GetScrollNotification()
        {
            var scrollNotification = await _dbContext.ScrollNotifications.ToListAsync();
            if (scrollNotification != null)
                return scrollNotification;
            return scrollNotification;
        }

        public async Task<List<DocumentHistoryView>> GetDocumentHistoryById(string invoiceNumber)
        {
            var header = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(t => t.ODIN == invoiceNumber || t.INV_NO == invoiceNumber);
            if (header == null)
            {
                throw new Exception("Invoice not found");
            }
            else
            {
                var documentHistories = await _dbContext.DocumentHistories.Where(t => t.HeaderId == header.HEADER_ID).Select(doc => new DocumentHistoryView
                {
                    Id = doc.Id,
                    FileName = doc.FileName,
                    FileType = doc.FileType,
                    CreatedBy = (from tb in _dbContext.Users where tb.UserID == new Guid(doc.CreatedBy) select tb.UserName).FirstOrDefault(),
                    CreatedOn = doc.CreatedOn
                }).ToListAsync();
                return documentHistories;
            }
        }

        public async Task<byte[]> DowloandHistoryDocument(int id)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                var att = await _dbContext.DocumentHistories.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (att != null && att.FileContent.Length > 0)
                {
                    byte[] bytes = att.FileContent;
                    var statuscode = HttpStatusCode.OK;
                    response.Content = new ByteArrayContent(bytes);
                    response.Content.Headers.Add("x-filename", att.FileName);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = att.FileName
                    };
                    return bytes.ToArray();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AttachmentResponse> DowloadPODDocument(int attachmentID)
        {
            try
            {
                P_INV_ATTACHMENT p_INV_ATTACHMENT = await (_dbContext.P_INV_ATTACHMENT).Where(x => x.ATTACHMENT_ID == attachmentID).FirstOrDefaultAsync();
                if (p_INV_ATTACHMENT != null && p_INV_ATTACHMENT.ATTACHMENT_FILE.Length != 0)
                {
                    return new AttachmentResponse()
                    {
                        FileName = p_INV_ATTACHMENT.ATTACHMENT_NAME,
                        FileContent = p_INV_ATTACHMENT.ATTACHMENT_FILE,
                        Extension = p_INV_ATTACHMENT.DOCUMENT_TYPE
                    };
                }
                else
                    throw new Exception("No Attachment found.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region SAP integration

        public async Task<InsertInvoiceResponse> InsertInvoiceDetails(InsertInvoiceDetail insertInvoiceDetail)
        {
            var response = new InsertInvoiceResponse();
            response.Status = true;
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 20)) + "INVOICE INSERT PROCESS STARTED" + String.Concat(Enumerable.Repeat("*", 20)));
                    LogWriter.WriteProcessLog($"ForwardLogisticsRepository/InsertInvoiceDetails - Payload :- {JsonConvert.SerializeObject(insertInvoiceDetail)}");

                    P_INV_HEADER_DETAIL invHeader = _dbContext.P_INV_HEADER_DETAIL.Where(x => x.INV_NO == insertInvoiceDetail.INV_NO &&
                      x.CUSTOMER == insertInvoiceDetail.CUSTOMER && x.ODIN == insertInvoiceDetail.ODIN && x.IS_ACTIVE).FirstOrDefault();

                    if (invHeader == null)
                    {
                        var header = CreateInvHeader(insertInvoiceDetail);
                        await _dbContext.P_INV_HEADER_DETAIL.AddAsync(header);
                        await _dbContext.SaveChangesAsync();

                        var items = CreateInvoiceLineItem(header.HEADER_ID, insertInvoiceDetail.ITEM_LIST);
                        await _dbContext.P_INV_ITEM_DETAIL.AddRangeAsync(items);
                        await _dbContext.SaveChangesAsync();
                        response.StatusMessage = $"Invoice No {header.INV_NO} is inserted";
                    }
                    else if (invHeader.STATUS == "Open")
                    {
                        UpdateInvHeader(invHeader, insertInvoiceDetail);
                        await _dbContext.SaveChangesAsync();
                        var olditems = _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == invHeader.HEADER_ID).ToList();
                        _dbContext.P_INV_ITEM_DETAIL.RemoveRange(olditems);
                        await _dbContext.SaveChangesAsync();
                        var items = CreateInvoiceLineItem(invHeader.HEADER_ID, insertInvoiceDetail.ITEM_LIST);
                        await _dbContext.P_INV_ITEM_DETAIL.AddRangeAsync(items);
                        await _dbContext.SaveChangesAsync();
                        response.StatusMessage = $"Invoice No {invHeader.INV_NO} is updated";
                    }
                    else
                    {
                        LogWriter.WriteProcessLog($"Invoice No {invHeader.INV_NO} already exist with the status {invHeader.STATUS}");
                        response.Status = false;
                        response.StatusMessage = $"Invoice No {invHeader.INV_NO} already exist with the status {invHeader.STATUS}";
                    }
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 50)));
                    await transaction.CommitAsync();
                    return response;
                }
                catch (Exception ex)
                {
                    LogWriter.WriteProcessLog("ForwardLogisticsRepository/InsertInvoiceDetails/Exception", ex);
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 50)));
                    response.Status = false;
                    response.StatusMessage = ex.Message;
                    await transaction.RollbackAsync();
                    return response;
                }
            }
        }

        private P_INV_HEADER_DETAIL CreateInvHeader(InsertInvoiceDetail insertInvoiceDetail)
        {

            P_INV_HEADER_DETAIL p_INV_HEADER_DETAIL = new P_INV_HEADER_DETAIL();
            p_INV_HEADER_DETAIL.PLANT = insertInvoiceDetail.PLANT;
            p_INV_HEADER_DETAIL.PLANT_NAME = insertInvoiceDetail.PLANT_NAME;
            p_INV_HEADER_DETAIL.INV_NO = insertInvoiceDetail.INV_NO;
            p_INV_HEADER_DETAIL.ODIN = insertInvoiceDetail.ODIN;
            DateTime iDate;
            if (DateTime.TryParse(insertInvoiceDetail.INV_DATE, out iDate))
            {
                p_INV_HEADER_DETAIL.INV_DATE = iDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.INV_DATE = (DateTime?)null;
            }
            p_INV_HEADER_DETAIL.INV_TYPE = insertInvoiceDetail.INV_TYPE;
            p_INV_HEADER_DETAIL.CUSTOMER = insertInvoiceDetail.CUSTOMER;
            p_INV_HEADER_DETAIL.CUSTOMER_NAME = insertInvoiceDetail.CUSTOMER_NAME;
            p_INV_HEADER_DETAIL.ORGANIZATION = insertInvoiceDetail.ORGANIZATION;
            p_INV_HEADER_DETAIL.DIVISION = insertInvoiceDetail.DIVISION;
            p_INV_HEADER_DETAIL.VEHICLE_NO = insertInvoiceDetail.VEHICLE_NO;
            p_INV_HEADER_DETAIL.VEHICLE_CAPACITY = insertInvoiceDetail.VEHICLE_CAPACITY;
            p_INV_HEADER_DETAIL.EWAYBILL_NO = insertInvoiceDetail.EWAYBILL_NO;
            DateTime eDate;
            if (DateTime.TryParse(insertInvoiceDetail.EWAYBILL_DATE, out eDate))
            {
                p_INV_HEADER_DETAIL.EWAYBILL_DATE = eDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.EWAYBILL_DATE = (DateTime?)null;
            }
            p_INV_HEADER_DETAIL.LR_NO = insertInvoiceDetail.LR_NO;
            DateTime lrDate;
            if (DateTime.TryParse(insertInvoiceDetail.LR_DATE, out lrDate))
            {
                p_INV_HEADER_DETAIL.LR_DATE = lrDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.LR_DATE = (DateTime?)null;
            }
            p_INV_HEADER_DETAIL.FWD_AGENT = insertInvoiceDetail.FWD_AGENT;
            p_INV_HEADER_DETAIL.CARRIER = insertInvoiceDetail.CARRIER;
            p_INV_HEADER_DETAIL.FREIGHT_ORDER = insertInvoiceDetail.FREIGHT_ORDER;
            DateTime frDate;
            if (DateTime.TryParse(insertInvoiceDetail.FREIGHT_ORDER_DATE, out frDate))
            {
                p_INV_HEADER_DETAIL.FREIGHT_ORDER_DATE = frDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.FREIGHT_ORDER_DATE = (DateTime?)null;
            }
            p_INV_HEADER_DETAIL.OUTBOUND_DELIVERY = insertInvoiceDetail.OUTBOUND_DELIVERY;
            DateTime oDate;
            if (DateTime.TryParse(insertInvoiceDetail.OUTBOUND_DELIVERY_DATE, out oDate))
            {
                p_INV_HEADER_DETAIL.OUTBOUND_DELIVERY_DATE = oDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.OUTBOUND_DELIVERY_DATE = (DateTime?)null;
            }
            DateTime adDate;
            if (DateTime.TryParse(insertInvoiceDetail.ACTUAL_DISPATCH_DATE, out adDate))
            {
                p_INV_HEADER_DETAIL.ACTUAL_DISPATCH_DATE = adDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.ACTUAL_DISPATCH_DATE = (DateTime?)null;
            }
            DateTime pDate;
            if (DateTime.TryParse(insertInvoiceDetail.PROPOSED_DELIVERY_DATE, out pDate))
            {
                p_INV_HEADER_DETAIL.PROPOSED_DELIVERY_DATE = pDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.PROPOSED_DELIVERY_DATE = (DateTime?)null;
            }
            DateTime dDate;
            if (DateTime.TryParse(insertInvoiceDetail.ACTUAL_DELIVERY_DATE, out dDate))
            {
                p_INV_HEADER_DETAIL.ACTUAL_DELIVERY_DATE = dDate;
            }
            else
            {
                p_INV_HEADER_DETAIL.ACTUAL_DELIVERY_DATE = (DateTime?)null;
            }
            p_INV_HEADER_DETAIL.TRANSIT_LEAD_TIME = insertInvoiceDetail.TRANSIT_LEAD_TIME;
            p_INV_HEADER_DETAIL.DISTANCE = insertInvoiceDetail.DISTANCE;
            p_INV_HEADER_DETAIL.CANC_INV_STATUS = insertInvoiceDetail.CANC_INV_STATUS;
            p_INV_HEADER_DETAIL.STATUS = "Open";
            p_INV_HEADER_DETAIL.STATUS_DESCRIPTION = "Invoice Details Inserted";
            p_INV_HEADER_DETAIL.CREATED_BY = "Admin";
            p_INV_HEADER_DETAIL.CREATED_ON = DateTime.Now;
            p_INV_HEADER_DETAIL.IS_ACTIVE = true;
            p_INV_HEADER_DETAIL.CUSTOMER_GROUP = insertInvoiceDetail.CUSTOMER_GROUP;
            p_INV_HEADER_DETAIL.CUSTOMER_DESTINATION = insertInvoiceDetail.CUSTOMER_DESTINATION;
            p_INV_HEADER_DETAIL.CUSTOMER_GROUP_DESC = insertInvoiceDetail.CUSTOMER_GROUP_DESC;
            p_INV_HEADER_DETAIL.PLANT_CODE = insertInvoiceDetail.PLANT_CODE;
            p_INV_HEADER_DETAIL.SECTOR_DESCRIPTION = insertInvoiceDetail.SECTOR_DESCRIPTION;
            p_INV_HEADER_DETAIL.GROSS_WEIGHT = insertInvoiceDetail.GROSS_WEIGHT;
            p_INV_HEADER_DETAIL.DRIVER_CONTACT = insertInvoiceDetail.DRIVER_CONTACT;

            p_INV_HEADER_DETAIL.TRACKING_LINK = "";
            p_INV_HEADER_DETAIL.TOTAL_TRAVEL_TIME = 0;
            p_INV_HEADER_DETAIL.TOTAL_DISTANCE = 0;
            p_INV_HEADER_DETAIL.DELIVERY_DATE = "";
            p_INV_HEADER_DETAIL.DELIVERY_TIME = "";

            return p_INV_HEADER_DETAIL;
        }

        private List<P_INV_ITEM_DETAIL> CreateInvoiceLineItem(int headerId, List<P_INV_ITEM_DETAIL> items)
        {
            items.ForEach(item =>
                        {
                            item.HEADER_ID = headerId;
                            item.STATUS = "Open";
                            item.STATUS_DESCRIPTION = "Item Inserted";
                            item.IS_ACTIVE = true;
                            item.CREATED_ON = DateTime.Now;
                        });
            return items;
        }

        private void UpdateInvHeader(P_INV_HEADER_DETAIL header, InsertInvoiceDetail insertInvoiceDetail)
        {
            header.PLANT = insertInvoiceDetail.PLANT;
            header.PLANT_NAME = insertInvoiceDetail.PLANT_NAME;
            header.INV_NO = insertInvoiceDetail.INV_NO;
            header.ODIN = insertInvoiceDetail.ODIN;
            DateTime iDate;
            if (DateTime.TryParse(insertInvoiceDetail.INV_DATE, out iDate))
            {
                header.INV_DATE = iDate;
            }
            else
            {
                header.INV_DATE = (DateTime?)null;
            }
            header.INV_TYPE = insertInvoiceDetail.INV_TYPE;
            header.CUSTOMER = insertInvoiceDetail.CUSTOMER;
            header.CUSTOMER_NAME = insertInvoiceDetail.CUSTOMER_NAME;
            header.ORGANIZATION = insertInvoiceDetail.ORGANIZATION;
            header.DIVISION = insertInvoiceDetail.DIVISION;
            header.VEHICLE_NO = insertInvoiceDetail.VEHICLE_NO;
            header.VEHICLE_CAPACITY = insertInvoiceDetail.VEHICLE_CAPACITY;
            header.EWAYBILL_NO = insertInvoiceDetail.EWAYBILL_NO;
            DateTime eDate;
            if (DateTime.TryParse(insertInvoiceDetail.EWAYBILL_DATE, out eDate))
            {
                header.EWAYBILL_DATE = eDate;
            }
            else
            {
                header.EWAYBILL_DATE = (DateTime?)null;
            }
            header.LR_NO = insertInvoiceDetail.LR_NO;
            DateTime lrDate;
            if (DateTime.TryParse(insertInvoiceDetail.LR_DATE, out lrDate))
            {
                header.LR_DATE = lrDate;
            }
            else
            {
                header.LR_DATE = (DateTime?)null;
            }
            header.FWD_AGENT = insertInvoiceDetail.FWD_AGENT;
            header.CARRIER = insertInvoiceDetail.CARRIER;
            header.FREIGHT_ORDER = insertInvoiceDetail.FREIGHT_ORDER;
            DateTime frDate;
            if (DateTime.TryParse(insertInvoiceDetail.FREIGHT_ORDER_DATE, out frDate))
            {
                header.FREIGHT_ORDER_DATE = frDate;
            }
            else
            {
                header.FREIGHT_ORDER_DATE = (DateTime?)null;
            }
            header.OUTBOUND_DELIVERY = insertInvoiceDetail.OUTBOUND_DELIVERY;
            DateTime oDate;
            if (DateTime.TryParse(insertInvoiceDetail.OUTBOUND_DELIVERY_DATE, out oDate))
            {
                header.OUTBOUND_DELIVERY_DATE = oDate;
            }
            else
            {
                header.OUTBOUND_DELIVERY_DATE = (DateTime?)null;
            }
            DateTime adDate;
            if (DateTime.TryParse(insertInvoiceDetail.ACTUAL_DISPATCH_DATE, out adDate))
            {
                header.ACTUAL_DISPATCH_DATE = adDate;
            }
            else
            {
                header.ACTUAL_DISPATCH_DATE = (DateTime?)null;
            }
            DateTime pDate;
            if (DateTime.TryParse(insertInvoiceDetail.PROPOSED_DELIVERY_DATE, out pDate))
            {
                header.PROPOSED_DELIVERY_DATE = pDate;
            }
            else
            {
                header.PROPOSED_DELIVERY_DATE = (DateTime?)null;
            }
            header.TRANSIT_LEAD_TIME = insertInvoiceDetail.TRANSIT_LEAD_TIME;
            header.DISTANCE = insertInvoiceDetail.DISTANCE;
            header.CANC_INV_STATUS = insertInvoiceDetail.CANC_INV_STATUS;
            header.STATUS = "Open";
            header.STATUS_DESCRIPTION = "Invoice Details Updated";
            header.CREATED_BY = "Admin";
            header.CREATED_ON = DateTime.Now;
            header.IS_ACTIVE = true;
            header.CUSTOMER_GROUP = insertInvoiceDetail.CUSTOMER_GROUP;
            header.CUSTOMER_DESTINATION = insertInvoiceDetail.CUSTOMER_DESTINATION;
            header.CUSTOMER_GROUP_DESC = insertInvoiceDetail.CUSTOMER_GROUP_DESC;
            header.PLANT_CODE = insertInvoiceDetail.PLANT_CODE;
            header.SECTOR_DESCRIPTION = insertInvoiceDetail.SECTOR_DESCRIPTION;
            header.GROSS_WEIGHT = insertInvoiceDetail.GROSS_WEIGHT;
            header.DRIVER_CONTACT = insertInvoiceDetail.DRIVER_CONTACT;

            header.TRACKING_LINK = "";
            header.TOTAL_TRAVEL_TIME = 0;
            header.TOTAL_DISTANCE = 0;
            header.DELIVERY_DATE = "";
            header.DELIVERY_TIME = "";
        }


        #endregion


        #region Acknowledgment

        //invoice screen - confirm
        public async Task<ResponseMessage> ConfirmInvoice()
        {
            var response = new ResponseMessage()
            {
                Status = 200,
                Message = "",
                Error = ""
            };
            LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 25)));
            string path = _networkCredential.ForwardAttachmentsPath;
            string sharedFolderUserName = _networkCredential.SharedFolderUserName;
            string sharedFolderPassword = _networkCredential.SharedFolderPassword;
            string sharedFolderDomain = _networkCredential.SharedFolderDomain;
            string fileName = "";
            using (var transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    InvoiceUpdate invoiceUpdate = JsonConvert.DeserializeObject<InvoiceUpdate>(request.Form["InvoiceUpdate"]);
                    IFormFileCollection postedfiles = request.Form.Files;
                    if (postedfiles.Count > 0)
                    {
                        var ext = postedfiles[0].FileName.ToString().Split('.')[postedfiles[0].FileName.ToString().Split('.').Length - 1];
                        fileName = invoiceUpdate.HEADER_ID.ToString().Replace("/", "_") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "") + "." + ext;
                        using (Stream st = postedfiles[0].OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPDF(fileName, fileBytes);
                                    fileName = convertedAttachment.Filename;
                                    string fullPath = Path.Combine(path, convertedAttachment.Filename);
                                    try
                                    {
                                        using (var impersonationHelper = new ImpersonationHelper(sharedFolderUserName, sharedFolderDomain, sharedFolderPassword))
                                        {
                                            File.WriteAllBytes(fullPath, convertedAttachment.PDFcontent);
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
                                    var header = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID && x.IS_ACTIVE);
                                    if (header != null)
                                    {
                                        var plGrps = _dbContext.PlantGroupPlantMaps.Where(x => x.PlantGroupId == 4).Select(p => p.PlantCode).ToList();
                                        if (plGrps != null && plGrps.Count > 0 && plGrps.Contains(header.PLANT_CODE))
                                        {
                                            var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID);
                                            if (previousAttachment == null)
                                            {
                                                P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                                attachment.HEADER_ID = invoiceUpdate.HEADER_ID;
                                                attachment.FILE_NAME = convertedAttachment.Filename;
                                                attachment.DOCUMENT_TYPE = "application/pdf";
                                                attachment.FILE_PATH = fullPath;
                                                attachment.CREATED_BY = invoiceUpdate.UserId;
                                                attachment.CREATED_ON = DateTime.Now;
                                                attachment.IS_ACTIVE = true;
                                                _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var docHistory = new DocumentHistory();
                                                docHistory.HeaderId = previousAttachment.HEADER_ID;
                                                docHistory.FileName = previousAttachment.FILE_NAME;
                                                docHistory.FilePath = previousAttachment.FILE_PATH;
                                                docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                                docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                                docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                                _dbContext.DocumentHistories.Add(docHistory);
                                                previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                                previousAttachment.FILE_PATH = fullPath;
                                                previousAttachment.CREATED_ON = DateTime.Now;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            var headerList = _dbContext.P_INV_HEADER_DETAIL.Where(t => t.LR_NO == header.LR_NO && t.CUSTOMER == header.CUSTOMER && t.LR_DATE == header.LR_DATE).Select(t => t.HEADER_ID).ToList();
                                            foreach (var headerId in headerList)
                                            {
                                                var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == headerId);

                                                if (previousAttachment == null)
                                                {
                                                    P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                                    attachment.HEADER_ID = headerId;
                                                    attachment.FILE_NAME = convertedAttachment.Filename;
                                                    attachment.DOCUMENT_TYPE = "application/pdf";
                                                    attachment.FILE_PATH = fullPath;
                                                    attachment.CREATED_BY = invoiceUpdate.UserId;
                                                    attachment.CREATED_ON = DateTime.Now;
                                                    attachment.IS_ACTIVE = true;
                                                    _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    var docHistory = new DocumentHistory();
                                                    docHistory.HeaderId = previousAttachment.HEADER_ID;
                                                    docHistory.FileName = previousAttachment.FILE_NAME;
                                                    docHistory.FilePath = previousAttachment.FILE_PATH;
                                                    docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                                    docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                                    docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                                    _dbContext.DocumentHistories.Add(docHistory);
                                                    previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                                    previousAttachment.FILE_PATH = fullPath;
                                                    previousAttachment.CREATED_ON = DateTime.Now;
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                            }
                                        }

                                        List<P_INV_ITEM_DETAIL> InvoiceItems = _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == invoiceUpdate.HEADER_ID).ToList();
                                        InvoiceItems.ForEach((item) =>
                                        {
                                            item.RECEIVED_QUANTITY = item.QUANTITY;
                                            item.STATUS = "Confirmed";
                                        });

                                        P_INV_HEADER_DETAIL head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID && x.IS_ACTIVE);
                                        if (head != null)
                                        {
                                            head.STATUS = "Confirmed";
                                            head.ACTUAL_DELIVERY_DATE = DateTime.Now;
                                        }
                                        await _dbContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        response.Message = $"Invoice {head.INV_NO} Confirmed";
                                        return response;
                                    }
                                    else
                                    {
                                        throw new Exception("Unable to find the invoice.");
                                    }
                                }
                            }
                        }
                    }
                    throw new Exception("Please Attach file.");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 25)));
                    throw;
                }
            }
        }

        //invoice-line item screen - confirm
        public async Task<ResponseMessage> ConfirmInvoiceDetails()
        {
            var response = new ResponseMessage()
            {
                Status = 200,
                Message = "",
                Error = ""
            };
            string path = _networkCredential.ForwardAttachmentsPath;
            string sharedFolderUserName = _networkCredential.SharedFolderUserName;
            string sharedFolderPassword = _networkCredential.SharedFolderPassword;
            string sharedFolderDomain = _networkCredential.SharedFolderDomain;
            string fileName = "";
            LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 25)));
            using (var transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    InvoiceUpdate invoiceUpdate = JsonConvert.DeserializeObject<InvoiceUpdate>(request.Form["InvoiceUpdate"]);
                    IFormFileCollection postedfiles = request.Form.Files;
                    if (postedfiles.Count > 0)
                    {
                        var ext = postedfiles[0].FileName.ToString().Split('.')[postedfiles[0].FileName.ToString().Split('.').Length - 1];
                        fileName = invoiceUpdate.HEADER_ID.ToString().Replace("/", "_") + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "") + "." + ext;
                        using (Stream st = postedfiles[0].OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPDF(fileName, fileBytes);
                                    fileName = convertedAttachment.Filename;
                                    string fullPath = Path.Combine(path, convertedAttachment.Filename);
                                    try
                                    {
                                        using (var impersonationHelper = new ImpersonationHelper(sharedFolderUserName, sharedFolderDomain, sharedFolderPassword))
                                        {
                                            File.WriteAllBytes(fullPath, convertedAttachment.PDFcontent);
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
                                    var header = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID && x.IS_ACTIVE);
                                    if (header != null)
                                    {
                                        var plGrps = _dbContext.PlantGroupPlantMaps.Where(x => x.PlantGroupId == 4).Select(p => p.PlantCode).ToList();
                                        if (plGrps != null && plGrps.Count > 0 && plGrps.Contains(header.PLANT_CODE))
                                        {
                                            var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID);
                                            if (previousAttachment == null)
                                            {
                                                P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                                attachment.HEADER_ID = invoiceUpdate.HEADER_ID;
                                                attachment.FILE_NAME = convertedAttachment.Filename;
                                                attachment.DOCUMENT_TYPE = "application/pdf";
                                                attachment.FILE_PATH = fullPath;
                                                attachment.CREATED_BY = invoiceUpdate.UserId;
                                                attachment.CREATED_ON = DateTime.Now;
                                                attachment.IS_ACTIVE = true;
                                                _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var docHistory = new DocumentHistory();
                                                docHistory.HeaderId = previousAttachment.HEADER_ID;
                                                docHistory.FileName = previousAttachment.FILE_NAME;
                                                docHistory.FilePath = previousAttachment.FILE_PATH;
                                                docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                                docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                                docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                                _dbContext.DocumentHistories.Add(docHistory);
                                                previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                                previousAttachment.FILE_PATH = fullPath;
                                                previousAttachment.CREATED_ON = DateTime.Now;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                        else
                                        {
                                            var headerList = _dbContext.P_INV_HEADER_DETAIL.Where(t => t.LR_NO == header.LR_NO && t.CUSTOMER == header.CUSTOMER && t.LR_DATE == header.LR_DATE).Select(t => t.HEADER_ID).ToList();
                                            foreach (var headerId in headerList)
                                            {
                                                var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == headerId);

                                                if (previousAttachment == null)
                                                {
                                                    P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                                    attachment.HEADER_ID = headerId;
                                                    attachment.FILE_NAME = convertedAttachment.Filename;
                                                    attachment.DOCUMENT_TYPE = "application/pdf";
                                                    attachment.FILE_PATH = fullPath;
                                                    attachment.CREATED_BY = invoiceUpdate.UserId;
                                                    attachment.CREATED_ON = DateTime.Now;
                                                    attachment.IS_ACTIVE = true;
                                                    _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                                    await _dbContext.SaveChangesAsync();
                                                }
                                                else
                                                {
                                                    var docHistory = new DocumentHistory();
                                                    docHistory.HeaderId = previousAttachment.HEADER_ID;
                                                    docHistory.FileName = previousAttachment.FILE_NAME;
                                                    docHistory.FilePath = previousAttachment.FILE_PATH;
                                                    docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                                    docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                                    docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                                    _dbContext.DocumentHistories.Add(docHistory);
                                                    previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                                    previousAttachment.FILE_PATH = fullPath;
                                                    previousAttachment.CREATED_ON = DateTime.Now;
                                                    await _dbContext.SaveChangesAsync();
                                                }

                                            }
                                        }

                                        bool isPartiallyConfirmed = false;
                                        foreach (var item in invoiceUpdate.InvoiceItems)
                                        {
                                            var existing = _dbContext.P_INV_ITEM_DETAIL.FirstOrDefault(x => x.ITEM_ID == item.ITEM_ID && x.HEADER_ID == item.HEADER_ID);
                                            if (existing != null)
                                            {
                                                existing.RECEIVED_QUANTITY = item.RECEIVED_QUANTITY;
                                                if (item.RECEIVED_QUANTITY < item.QUANTITY)
                                                {
                                                    existing.STATUS = "PartiallyConfirmed";
                                                    isPartiallyConfirmed = true;
                                                }
                                                else
                                                    existing.STATUS = "Confirmed";
                                            }
                                        }

                                        var head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID && x.IS_ACTIVE);
                                        if (head != null)
                                        {
                                            head.STATUS = isPartiallyConfirmed ? "PartiallyConfirmed" : "Confirmed";
                                            head.ACTUAL_DELIVERY_DATE = DateTime.Now;
                                        }

                                        await _dbContext.SaveChangesAsync();
                                        await transaction.CommitAsync();
                                        response.Message = $"Invoice {head.INV_NO} Confirmed";
                                        return response;
                                    }
                                    else
                                    {
                                        throw new Exception("Unable to find the invoice.");
                                    }
                                }
                            }
                        }
                    }
                    throw new Exception("Please Attach file.");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 25)));
                    throw;
                }
            }
        }

        //confirm-qty from both invoice & invoice-line item screens. Update invoice-items only with (date, headerID)
        public async Task<ResponseMessage> ConfirmQty(InvoiceUpdate invoiceUpdate)
        {
            var response = new ResponseMessage()
            {
                Status = 200,
                Message = "",
                Error = ""
            };

            using (var transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    List<P_INV_ITEM_DETAIL> InvoiceItems = _dbContext.P_INV_ITEM_DETAIL.Where(x => x.HEADER_ID == invoiceUpdate.HEADER_ID).ToList();
                    InvoiceItems.ForEach((item) =>
                    {
                        item.RECEIVED_QUANTITY = item.QUANTITY;
                        item.STATUS = "Confirmed";
                    });

                    P_INV_HEADER_DETAIL head = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == invoiceUpdate.HEADER_ID && x.IS_ACTIVE);
                    if (head != null)
                    {
                        head.STATUS = "Confirmed";
                        head.VEHICLE_REPORTED_DATE = invoiceUpdate.VEHICLE_REPORTED_DATE;
                        head.ACTUAL_DELIVERY_DATE = DateTime.Now;
                    }
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    response.Message = $"Invoice {head.INV_NO} Confirmed";
                    return response;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }


        //re-upload from both invoice & invoice-line item screens. Update invoice with (form , attachment, HEaderID)
        public async Task<bool> ReUploadLR()
        {
            var response = new ResponseMessage()
            {
                Status = 200,
                Message = "",
                Error = ""
            };
            string path = _networkCredential.ForwardAttachmentsPath;
            string sharedFolderUserName = _networkCredential.SharedFolderUserName;
            string sharedFolderPassword = _networkCredential.SharedFolderPassword;
            string sharedFolderDomain = _networkCredential.SharedFolderDomain;
            string fileName = "";

            //string fileName = $"{headerId}_{DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "")}.pdf";
            using (var transaction = await this._dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    int headerID = int.Parse(request.Form["HeaderID"].ToString());
                    string userID = request.Form["UserID"].ToString();
                    var header = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(x => x.HEADER_ID == headerID && x.IS_ACTIVE);
                    IFormFileCollection postedfiles = request.Form.Files;
                    if (postedfiles.Count > 0)
                    {
                        var ext = postedfiles[0].FileName.ToString().Split('.')[postedfiles[0].FileName.ToString().Split('.').Length - 1];
                        fileName = header.HEADER_ID.ToString().Replace("/", "_") + "_" + header.CUSTOMER + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "") + "." + ext;
                        using (Stream st = postedfiles[0].OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPDF(fileName, fileBytes);
                                    fileName = convertedAttachment.Filename;
                                    string fullPath = Path.Combine(path, convertedAttachment.Filename);
                                    try
                                    {
                                        using (var impersonationHelper = new ImpersonationHelper(sharedFolderUserName, sharedFolderDomain, sharedFolderPassword))
                                        {
                                            File.WriteAllBytes(fullPath, convertedAttachment.PDFcontent);
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

                                    var plGrps = _dbContext.PlantGroupPlantMaps.Where(x => x.PlantGroupId == 4).Select(p => p.PlantCode).ToList();
                                    if (plGrps != null && plGrps.Count > 0 && plGrps.Contains(header.PLANT_CODE))
                                    {
                                        var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == headerID);
                                        if (previousAttachment == null)
                                        {
                                            P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                            attachment.HEADER_ID = headerID;
                                            attachment.FILE_NAME = convertedAttachment.Filename;
                                            attachment.DOCUMENT_TYPE = "application/pdf";
                                            attachment.FILE_PATH = fullPath;
                                            attachment.CREATED_BY = userID;
                                            attachment.CREATED_ON = DateTime.Now;
                                            attachment.IS_ACTIVE = true;
                                            _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                            await _dbContext.SaveChangesAsync();
                                        }
                                        else
                                        {
                                            var docHistory = new DocumentHistory();
                                            docHistory.HeaderId = previousAttachment.HEADER_ID;
                                            docHistory.FileName = previousAttachment.FILE_NAME;
                                            docHistory.FilePath = previousAttachment.FILE_PATH;
                                            docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                            docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                            docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                            _dbContext.DocumentHistories.Add(docHistory);
                                            previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                            previousAttachment.FILE_PATH = fullPath;
                                            previousAttachment.CREATED_ON = DateTime.Now;
                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                    else
                                    {
                                        var headerList = _dbContext.P_INV_HEADER_DETAIL.Where(t => t.LR_NO == header.LR_NO && t.CUSTOMER == header.CUSTOMER && t.LR_DATE == header.LR_DATE).Select(t => t.HEADER_ID).ToList();
                                        foreach (var id in headerList)
                                        {
                                            var previousAttachment = _dbContext.P_INV_ATTACHMENT.FirstOrDefault(x => x.HEADER_ID == id);

                                            if (previousAttachment == null)
                                            {
                                                P_INV_ATTACHMENT attachment = new P_INV_ATTACHMENT();
                                                attachment.HEADER_ID = id;
                                                attachment.FILE_NAME = convertedAttachment.Filename;
                                                attachment.DOCUMENT_TYPE = "application/pdf";
                                                attachment.FILE_PATH = fullPath;
                                                attachment.CREATED_BY = userID;
                                                attachment.CREATED_ON = DateTime.Now;
                                                attachment.IS_ACTIVE = true;
                                                _dbContext.P_INV_ATTACHMENT.Add(attachment);
                                                await _dbContext.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                var docHistory = new DocumentHistory();
                                                docHistory.HeaderId = previousAttachment.HEADER_ID;
                                                docHistory.FileName = previousAttachment.FILE_NAME;
                                                docHistory.FilePath = previousAttachment.FILE_PATH;
                                                docHistory.FileType = previousAttachment.DOCUMENT_TYPE;
                                                docHistory.CreatedOn = previousAttachment.CREATED_ON;
                                                docHistory.CreatedBy = previousAttachment.CREATED_BY;
                                                _dbContext.DocumentHistories.Add(docHistory);
                                                previousAttachment.FILE_NAME = convertedAttachment.Filename;
                                                previousAttachment.FILE_PATH = fullPath;
                                                previousAttachment.CREATED_ON = DateTime.Now;
                                                await _dbContext.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    LogWriter.WriteProcessLog(String.Concat(Enumerable.Repeat("*", 25)));
                    throw;
                }
            }
        }

        #endregion


        #region Sales Return
        public async Task<string> SalesReturns(SalesReturnProps salesReturnProps)
        {
            try
            {
                P_INV_HEADER_DETAIL inv_header = await _dbContext.P_INV_HEADER_DETAIL.FirstOrDefaultAsync(t => t.INV_NO == salesReturnProps.InvoiceNumber && t.IS_ACTIVE);
                if (inv_header == null)
                {
                    throw new Exception($"Invoice not found!!!");
                }
                else
                {
                    var item_details = await _dbContext.P_INV_ITEM_DETAIL.Where(t => t.HEADER_ID == inv_header.HEADER_ID).ToListAsync();
                    foreach (var material in salesReturnProps.SalesReturnMaterial)
                    {
                        var materialItem = item_details.FirstOrDefault(t => t.MATERIAL_CODE == material.MaterialCode && t.ITEM_NO == material.ItemNumber);
                        if (materialItem != null)
                        {
                            if (materialItem.RECEIVED_QUANTITY == null)
                            {
                                materialItem.RECEIVED_QUANTITY = 0;
                            }
                            var creditInvoices = _dbContext.SalesReturnCreditNoteLogs.Where(t => t.InvoiceNumber == salesReturnProps.InvoiceNumber && t.MaterialCode == material.MaterialCode && t.ItemNumber == material.ItemNumber).ToList();
                            var totalQty = materialItem.RECEIVED_QUANTITY + material.Qty;
                            foreach (var CI in creditInvoices)
                            {
                                totalQty += CI.Qty;
                            }
                            if (totalQty > materialItem.QUANTITY)
                            {
                                materialItem.REMARKS = String.IsNullOrEmpty(materialItem.REMARKS) ? "CI" + salesReturnProps.CreditInvoice + $"({material.Qty}) - Recieved quantity is greater than invoice quantity!!" : materialItem.REMARKS + ",CI" + salesReturnProps.CreditInvoice + $"({material.Qty}) - Recieved quantity is greater than invoice quantity!!";
                            }
                            else
                            {
                                materialItem.REMARKS = String.IsNullOrEmpty(materialItem.REMARKS) ? "CI" + salesReturnProps.CreditInvoice + $"({material.Qty})" : materialItem.REMARKS + ",CI" + salesReturnProps.CreditInvoice + $"({material.Qty})";
                            }

                            SalesReturnCreditNoteLog salesReturnCreditNoteLog = new SalesReturnCreditNoteLog();
                            salesReturnCreditNoteLog.CreditInvoice = salesReturnProps.CreditInvoice;
                            salesReturnCreditNoteLog.InvoiceNumber = salesReturnProps.InvoiceNumber;
                            salesReturnCreditNoteLog.ItemNumber = materialItem.ITEM_NO;
                            salesReturnCreditNoteLog.MaterialCode = material.MaterialCode;
                            salesReturnCreditNoteLog.Qty = material.Qty;
                            salesReturnCreditNoteLog.IsActive = true;
                            _dbContext.SalesReturnCreditNoteLogs.Add(salesReturnCreditNoteLog);
                            await _dbContext.SaveChangesAsync();
                        }
                        else
                        {
                            throw new Exception($"PODConfirmation/SalesReturns: Material - {material.MaterialCode} Not Found!!!");
                        }
                    }
                    var tCreditInvoices = await _dbContext.SalesReturnCreditNoteLogs.Where(t => t.InvoiceNumber == salesReturnProps.InvoiceNumber).GroupBy(t => t.ItemNumber).ToListAsync();
                    var slConfirmationList = new List<SalesReturnStatusConfirmation>();
                    foreach (var tCI in tCreditInvoices)
                    {
                        var slConfirm = new SalesReturnStatusConfirmation();
                        var mItem = item_details.FirstOrDefault(t => t.ITEM_NO == tCI.Key);
                        slConfirm.Quantity = mItem.QUANTITY;
                        slConfirm.RecievedQuantity = tCI.Sum(t => t.Qty) + mItem.RECEIVED_QUANTITY;
                        slConfirmationList.Add(slConfirm);
                    }
                    if (inv_header.STATUS != "Confirmed")
                    {
                        inv_header.STATUS = slConfirmationList.All(k => k.Quantity == k.RecievedQuantity) ? "Confirmed" : "PartiallyConfirmed";
                    }
                    if (!inv_header.ACTUAL_DELIVERY_DATE.HasValue)
                    {
                        inv_header.ACTUAL_DELIVERY_DATE = DateTime.Now;
                    }

                    await _dbContext.SaveChangesAsync();
                    return "Status changed in Database, success!";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> CancelSalesReturn(string CRinvoiceno)
        {
            try
            {
                var slslog = await (from tb in _dbContext.SalesReturnCreditNoteLogs
                                    join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.InvoiceNumber equals tb1.INV_NO
                                    join tb2 in _dbContext.P_INV_ITEM_DETAIL on tb1.HEADER_ID equals tb2.HEADER_ID
                                    where tb.CreditInvoice == CRinvoiceno && tb2.MATERIAL_CODE == tb.MaterialCode && tb.ItemNumber == tb2.ITEM_NO
                                    select new
                                    {
                                        tb2.ITEM_ID,
                                        tb.CRID,
                                        tb2.MATERIAL_CODE,
                                        tb2.QUANTITY,
                                        tb2.RECEIVED_QUANTITY,
                                        tb.CreditInvoice,
                                        tb.Qty,
                                        tb.IsActive
                                    }
                              ).FirstOrDefaultAsync();

                if (slslog != null)
                {
                    var itm = await _dbContext.P_INV_ITEM_DETAIL.Where(k => k.ITEM_ID == slslog.ITEM_ID).FirstOrDefaultAsync();
                    var ls = itm.REMARKS.Split(',').ToList();

                    itm.RECEIVED_QUANTITY = itm.RECEIVED_QUANTITY - slslog.Qty;
                    for (int i = 0; i < ls.Count; i++)
                    {
                        if (ls[i].Contains(slslog.CreditInvoice))
                        {
                            ls.RemoveAt(i);
                        }
                    }

                    itm.REMARKS = String.Join(",", ls.ToArray());
                    _dbContext.SalesReturnCreditNoteLogs.Remove(_dbContext.SalesReturnCreditNoteLogs.FirstOrDefault(k => k.CRID == slslog.CRID));
                    await _dbContext.SaveChangesAsync();
                    return "Credit invoice cancelled!!!";
                }
                else
                {
                    throw new Exception("CI or Inv no not found ");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> InvoiceCancellation(string Invoiceno)
        {
            try
            {
                P_INV_HEADER_DETAIL result = await (from td in _dbContext.P_INV_HEADER_DETAIL
                                                    where td.INV_NO == Invoiceno && td.IS_ACTIVE
                                                    select td).FirstOrDefaultAsync();
                if (result != null)
                {
                    result.IS_ACTIVE = false;
                    await _dbContext.SaveChangesAsync();
                    return "Status changed in Database, success!";
                }
                else
                {

                    var slslog = await (from tb in _dbContext.SalesReturnCreditNoteLogs
                                        join tb1 in _dbContext.P_INV_HEADER_DETAIL on tb.InvoiceNumber equals tb1.INV_NO
                                        join tb2 in _dbContext.P_INV_ITEM_DETAIL on tb1.HEADER_ID equals tb2.HEADER_ID
                                        where tb.CreditInvoice == Invoiceno && tb2.MATERIAL_CODE == tb.MaterialCode && tb.ItemNumber == tb2.ITEM_NO
                                        select new
                                        {
                                            tb1.HEADER_ID,
                                            tb2.ITEM_ID,
                                            tb.CRID,
                                            tb2.MATERIAL_CODE,
                                            tb2.QUANTITY,
                                            tb2.RECEIVED_QUANTITY,
                                            tb.CreditInvoice,
                                            tb.Qty,
                                            tb.IsActive
                                        }
                              ).FirstOrDefaultAsync();

                    if (slslog != null)
                    {
                        var itm = await _dbContext.P_INV_ITEM_DETAIL.Where(k => k.ITEM_ID == slslog.ITEM_ID).FirstOrDefaultAsync();
                        var ls = itm.REMARKS.Split(',').ToList();

                        for (int i = 0; i < ls.Count; i++)
                        {
                            if (ls[i].Contains(slslog.CreditInvoice))
                            {
                                ls.RemoveAt(i);
                            }
                        }

                        itm.REMARKS = String.Join(",", ls.ToArray());
                        _dbContext.SalesReturnCreditNoteLogs.Remove(_dbContext.SalesReturnCreditNoteLogs.FirstOrDefault(k => k.CRID == slslog.CRID));
                        await _dbContext.SaveChangesAsync();
                        var header = _dbContext.P_INV_HEADER_DETAIL.FirstOrDefault(t => t.HEADER_ID == slslog.HEADER_ID);
                        if (header.VEHICLE_REPORTED_DATE.HasValue)
                        {
                            header.STATUS = "PartiallyConfirmed";
                        }
                        else
                        {
                            header.STATUS = "Open";
                        }
                        await _dbContext.SaveChangesAsync();
                        return "Credit invoice cancelled!!!";
                    }
                    else
                    {
                        throw new Exception("CI or Inv no not found ");
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
    }
}
