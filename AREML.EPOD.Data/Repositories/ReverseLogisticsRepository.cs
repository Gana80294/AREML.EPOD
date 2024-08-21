using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.ReverseLogistics;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AREML.EPOD.Data.Repositories
{
    public class ReverseLogisticsRepository : IReverseLogisticsRepository
    {

        private readonly AuthContext _dbContext;
        private readonly ExcelHelper _excelHelper;
        private readonly PdfCompresser _pdfCompresser;
        private readonly AppSetting _appSetting;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReverseLogisticsRepository(IConfiguration configuration, AuthContext authContext, ExcelHelper excel, PdfCompresser pdfCompresser, IHttpContextAccessor httpContextAccessor)
        {
            this._dbContext = authContext;
            this._excelHelper = excel;
            _pdfCompresser = pdfCompresser;
            _appSetting = configuration.GetSection("AppSettings").Get<AppSetting>();
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<List<ReversePodDetails>> GetAllReversePODByCustomer(string customerCode)
        {
            try
            {
                var result = await (from tb in _dbContext.RPOD_HEADER_DETAILS
                                    where tb.CUSTOMER == customerCode
                                    join tb1 in _dbContext.RPOD_LR_DETAILS on tb.RPOD_HEADER_ID equals tb1.RPOD_HEADER_ID into lrData
                                    select new ReversePodDetails
                                    {
                                        RPOD_HEADER_ID = tb.RPOD_HEADER_ID,
                                        DC_NUMBER = tb.DC_NUMBER,
                                        DC_DATE = tb.DC_DATE,
                                        CLAIM_TYPE = tb.CLAIM_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        SLA_DATE = tb.SLA_DATE,
                                        PENDING_DAYS = tb.PENDING_DAYS,
                                        STATUS = tb.STATUS,
                                        TOTAL_QUANTITY = tb.TOTAL_QUANTITY,
                                        BILLED_QUANTITY = tb.BILLED_QUANTITY,
                                        BALANCE_QUANTITY = tb.BALANCE_QUANTITY,
                                        LR_DETAILS = lrData.ToList()
                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<ReversePodMaterialDetails>> GetReverseMaterialDetails(int headerId)
        {
            try
            {
                var result = await (from tb in _dbContext.RPOD_MATERIAL_DETAILS
                                    join tb1 in _dbContext.RPOD_HEADER_DETAILS on tb.RPOD_HEADER_ID equals tb1.RPOD_HEADER_ID
                                    where tb.RPOD_HEADER_ID == headerId && tb1.IsActive
                                    join tb2 in _dbContext.WARRANTY_REPLACEMENT on tb.RPOD_HEADER_ID equals tb2.RPOD_HEADER_ID into warranty
                                    orderby tb.MATERIAL_ID
                                    select new ReversePodMaterialDetails
                                    {
                                        MATERIAL_ID = tb.MATERIAL_ID,
                                        RPOD_HEADER_ID = tb.RPOD_HEADER_ID,
                                        MATERIAL_CODE = tb.MATERIAL_CODE,
                                        QUANTITY = tb.QUANTITY,
                                        HAND_OVERED_QUANTITY = tb.HAND_OVERED_QUANTITY,
                                        CUSTOMER_PENDING_QUANTITY = tb.QUANTITY - tb.HAND_OVERED_QUANTITY,
                                        RECEIVED_QUANTITY = tb.RECEIVED_QUANTITY,
                                        DC_PENDING_QUANTITY = tb.HAND_OVERED_QUANTITY - tb.RECEIVED_QUANTITY,
                                        REMARKS = tb.REMARKS,
                                        STATUS = tb.STATUS,
                                        WARRANTY_REPLACEMENT_DETAILS = warranty.Where(x => x.MATERIAL_CODE == tb.MATERIAL_CODE).ToList()
                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RPOD_LR_DETAIL>> GetLRDetailsByHeaderId(int headerId)
        {
            try
            {
                var res = await (from tb in _dbContext.RPOD_LR_DETAILS
                                 where tb.RPOD_HEADER_ID == headerId
                                 select tb).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ReversePodDetails>> FilterReversePODDetails(ReversePodFilterClass filterClass)
        {
            try
            {
                var SkipValue = (filterClass.CurrentPage - 1) * filterClass.Records;
                var TakeValue = filterClass.Records;
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isCustomer = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isCustomerCode = !string.IsNullOrEmpty(filterClass.CustomerCode);
                bool isDcNum = !string.IsNullOrEmpty(filterClass.DcNo);

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }
                var result = await (from tb in _dbContext.RPOD_HEADER_DETAILS
                                    where
                                    (!isStatus || filterClass.Status.Contains(tb.STATUS)) &&
                                    (!isFromDate || (tb.SLA_DATE.HasValue && tb.SLA_DATE.Value.Date >= filterClass.StartDate.Value.Date))
                                    && (!isEndDate || (tb.SLA_DATE.HasValue && tb.SLA_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                      && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                      && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                      && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                      && (!isCustomerCode || tb.CUSTOMER == filterClass.CustomerCode)
                                      && (!isCustomer || tb.CUSTOMER.ToLower().Contains(filterClass.CustomerName.ToLower()))
                                      && (!isDcNum || tb.DC_NUMBER.ToLower() == filterClass.DcNo.ToLower())
                                    join tb1 in _dbContext.RPOD_LR_DETAILS on tb.RPOD_HEADER_ID equals tb1.RPOD_HEADER_ID into lrData
                                    orderby tb.RPOD_HEADER_ID
                                    select new ReversePodDetails
                                    {
                                        RPOD_HEADER_ID = tb.RPOD_HEADER_ID,
                                        DC_NUMBER = tb.DC_NUMBER,
                                        DC_DATE = tb.DC_DATE,
                                        CLAIM_TYPE = tb.CLAIM_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        SLA_DATE = tb.SLA_DATE,
                                        PENDING_DAYS = tb.PENDING_DAYS,
                                        STATUS = tb.STATUS,
                                        TOTAL_QUANTITY = tb.TOTAL_QUANTITY,
                                        BILLED_QUANTITY = tb.BILLED_QUANTITY,
                                        BALANCE_QUANTITY = tb.BALANCE_QUANTITY,
                                        LR_DETAILS = lrData.ToList(),
                                    }).Skip(SkipValue).Take(TakeValue).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<AttachmentResponse> DownloadRPODDocuments(int attachmentId)
        {
            try
            {
                string SharedFolderUserName = _appSetting.SharedFolderUserName;
                string SharedFolderPassword = _appSetting.SharedFolderPassword;
                string SharedFolderDomain = _appSetting.SharedFolderDomain;

                var att = await _dbContext.RPOD_LR_ATTACHMENTS.Where(x => x.Id == attachmentId).FirstOrDefaultAsync();
                if (att != null)
                {

                    try
                    {
                        using (var impersonationHelper = new ImpersonationHelper(SharedFolderUserName, SharedFolderDomain, SharedFolderPassword))
                        {
                            var fileContent = File.ReadAllBytes(att.FilePath);
                            var attResponse = new AttachmentResponse()
                            {
                                FileName = att.FileName,
                                FileContent = fileContent,
                                Extension = "application/pdf"
                            };
                            return attResponse;
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
                    throw new Exception("Unable to find the attachment");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<HttpResponseMessage> DownloadRPODReport(ReversePodFilterClass filterClass)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            CreateTempFolder();
            string TempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp");

            try
            {
                bool isStatus = filterClass.Status != null && filterClass.Status.Count > 0;
                bool isFromDate = filterClass.StartDate.HasValue;
                bool isEndDate = filterClass.EndDate.HasValue;
                bool isPlant = filterClass.PlantList != null && filterClass.PlantList.Count > 0;
                bool isPlantGroup = filterClass.PlantGroupList != null && filterClass.PlantGroupList.Count > 0;
                bool isCustomer = !string.IsNullOrEmpty(filterClass.CustomerName);
                bool isUserCode = !string.IsNullOrEmpty(filterClass.CustomerCode);

                List<string> plants = new List<string>();
                if (isPlantGroup)
                {
                    plants = await (from tb in _dbContext.PlantGroupPlantMaps
                                    join tb1 in _dbContext.PlantGroups on tb.PlantGroupId equals tb1.Id
                                    join tb2 in _dbContext.Plants on tb.PlantCode equals tb2.PlantCode
                                    where filterClass.PlantGroupList.Contains(tb1.name)
                                    select tb.PlantCode).Distinct().ToListAsync();
                }
                var result = await (from tb in _dbContext.RPOD_HEADER_DETAILS
                                    join tb1 in _dbContext.RPOD_MATERIAL_DETAILS on tb.RPOD_HEADER_ID equals tb1.RPOD_HEADER_ID
                                    where
                                   (!isFromDate || (tb.SLA_DATE.HasValue && tb.SLA_DATE.Value.Date >= (filterClass.StartDate.Value.Date))
                                    && (!isEndDate || (tb.SLA_DATE.HasValue && tb.SLA_DATE.Value.Date <= filterClass.EndDate.Value.Date))
                                    && (!isPlant || filterClass.PlantList.Any(x => x == tb.PLANT))
                                    && (!isPlantGroup || plants.Any(x => x == tb.PLANT))
                                    && (!isUserCode || tb.CUSTOMER == filterClass.CustomerCode)
                                    && (!isCustomer || tb.CUSTOMER.ToLower().Contains(filterClass.CustomerName.ToLower())))
                                    orderby tb.RPOD_HEADER_ID
                                    select new ReversePodReportDto
                                    {
                                        DC_NUMBER = tb.DC_NUMBER,
                                        DC_DATE = tb.DC_DATE,
                                        CLAIM_TYPE = tb.CLAIM_TYPE,
                                        CUSTOMER = tb.CUSTOMER,
                                        CUSTOMER_NAME = tb.CUSTOMER_NAME,
                                        PLANT = tb.PLANT,
                                        PLANT_NAME = tb.PLANT_NAME,
                                        SLA_DATE = tb.SLA_DATE,
                                        PENDING_DAYS = tb.PENDING_DAYS,
                                        STATUS = tb.STATUS,
                                        MATERIAL_CODE = tb1.MATERIAL_CODE,
                                        QUANTITY = tb1.QUANTITY,
                                        HAND_OVERED_QUANTITY = tb1.HAND_OVERED_QUANTITY,
                                        CUSTOMER_PENDING_QUANTITY = tb1.QUANTITY - tb1.HAND_OVERED_QUANTITY,
                                        RECEIVED_QUANTITY = tb1.RECEIVED_QUANTITY,
                                        DC_PENDING_QUANTITY = tb1.HAND_OVERED_QUANTITY - tb1.RECEIVED_QUANTITY,
                                        INVOICE_TOTAL_QTY = tb1.QUANTITY,
                                        INVOICE_BILLED_QTY = tb.BILLED_QUANTITY,
                                        INVOICE_BALANCE_QTY = tb.BALANCE_QUANTITY,
                                        REMARKS = tb1.REMARKS
                                    }).ToListAsync();


                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = _excelHelper.CreateNPOIWorksheet(result, workbook);
                DateTime dt1 = DateTime.Today;
                string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                var FileNm = $"Reverse POD details_{dtstr1}.xlsx";

                MemoryStream stream = new MemoryStream();

                workbook.Write(stream);
                byte[] fileByteArray = stream.ToArray();

                var statuscode = HttpStatusCode.OK;
                response = new HttpResponseMessage(statuscode);
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


        public async Task<bool> UpdateReversePodApprover(List<Guid> approvers)
        {
            try
            {
                var reversePodApprovers = await this._dbContext.ReversePodApprovers.ToListAsync();
                foreach (var approver in reversePodApprovers)
                {
                    approver.IsApprover = false;
                }
                await this._dbContext.SaveChangesAsync();
                foreach (var approverId in approvers)
                {
                    var existingApprover = await this._dbContext.ReversePodApprovers
                        .FirstOrDefaultAsync(x => x.UserId == approverId);

                    if (existingApprover != null)
                    {
                        existingApprover.IsApprover = true;
                    }
                    else
                    {
                        var newApprover = new ReversePodApprover
                        {
                            UserId = approverId,
                            IsApprover = true
                        };
                        this._dbContext.ReversePodApprovers.Add(newApprover);
                    }
                }
                await this._dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<ReversePodApprover> GetIsApprover(Guid UserId)
        {
            try
            {
                var res = await this._dbContext.ReversePodApprovers.FirstOrDefaultAsync(x => x.UserId == UserId);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Guid>> GetDcApprovers()
        {
            try
            {
                var res = await this._dbContext.ReversePodApprovers.Where(x => x.IsApprover).Select(y => y.UserId).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ConfirmReversePodDirectly()
        {

            string path = _appSetting.ReverseAttachmentsPath;
            string SharedFolderUserName = _appSetting.SharedFolderUserName;
            string SharedFolderPassword = _appSetting.SharedFolderPassword;
            string SharedFolderDomain = _appSetting.SharedFolderDomain;
            string fileName = "";

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    var payLoad = JsonConvert.DeserializeObject<ReversePodUpdation>(request.Form["Payload"].ToString());
                    var header = await this._dbContext.RPOD_HEADER_DETAILS.FirstOrDefaultAsync(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID);
                    IFormFileCollection postedfiles = request.Form.Files;
                    if (postedfiles.Count > 0)
                    {
                        var ext = postedfiles[0].FileName.ToString().Split('.')[postedfiles[0].FileName.ToString().Split('.').Length - 1];
                        fileName = header.DC_NUMBER.ToString().Replace("/", "_") + "_" + header.CUSTOMER + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "") + "." + ext;
                        using (Stream st = postedfiles[0].OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPdf(fileName, fileBytes);
                                    fileName = convertedAttachment.Filename;
                                    string fullPath = Path.Combine(path, convertedAttachment.Filename);
                                    try
                                    {
                                        using (var impersonationHelper = new ImpersonationHelper(SharedFolderUserName, SharedFolderDomain, SharedFolderPassword))
                                        {
                                            File.WriteAllBytes(Path.Combine(path, convertedAttachment.Filename), convertedAttachment.PDFcontent);
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

                                    RPOD_LR_ATTACHMENT lrDoc = new RPOD_LR_ATTACHMENT()
                                    {
                                        RPOD_HEADER_ID = payLoad.RPOD_HEADER_ID,
                                        Code = payLoad.Code,
                                        FileName = convertedAttachment.Filename,
                                        FilePath = fullPath,
                                        IsDeleted = false
                                    };

                                    if (payLoad.Code == 1)
                                    {
                                        header.STATUS = "In Transit";
                                    }
                                    else
                                    {
                                        header.STATUS = "Confirmed";
                                    }
                                    var materials = _dbContext.RPOD_MATERIAL_DETAILS.Where(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID);
                                    foreach (var item in materials)
                                    {
                                        item.HAND_OVERED_QUANTITY = item.QUANTITY;
                                        item.RECEIVED_QUANTITY = payLoad.Code == 2 ? item.QUANTITY : item.RECEIVED_QUANTITY;
                                        item.REMARKS = "";
                                        item.STATUS = "Confirmed";
                                    }

                                    _dbContext.RPOD_LR_ATTACHMENTS.Add(lrDoc);
                                    int attachStatus = 0;
                                    try
                                    {
                                        attachStatus = _dbContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                    if (attachStatus > 0)
                                    {
                                        var oldLr = await _dbContext.RPOD_LR_DETAILS.Where(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID).OrderByDescending(x => x.LR_ID).ToListAsync();
                                        if (oldLr.Count > 0 && oldLr[0].DC_DOC_ID == 0 && oldLr[0].DC_ACKNOWLEDGEMENT_DATE == null)
                                        {
                                            if (payLoad.Code == 1)
                                            {
                                                oldLr[0].CUSTOMER_DOC_ID = lrDoc.Id;
                                            }
                                            else
                                            {
                                                oldLr[0].DC_DOC_ID = lrDoc.Id;
                                                oldLr[0].DC_RECEIEVED_DATE = payLoad.DC_RECEIEVED_DATE;
                                                oldLr[0].DC_ACKNOWLEDGEMENT_DATE = DateTime.Now;
                                            }
                                        }
                                        else
                                        {
                                            var newLr = new RPOD_LR_DETAIL()
                                            {
                                                LR_NO = payLoad.LR_NO,
                                                LR_DATE = payLoad.LR_DATE,
                                                RPOD_HEADER_ID = payLoad.RPOD_HEADER_ID,
                                                CUSTOMER_DOC_ID = payLoad.Code == 1 ? lrDoc.Id : 0,
                                                DC_DOC_ID = payLoad.Code == 2 ? lrDoc.Id : 0,
                                                DC_RECEIEVED_DATE = payLoad.DC_RECEIEVED_DATE,
                                                DC_ACKNOWLEDGEMENT_DATE = payLoad.Code == 2 ? DateTime.Now : (DateTime?)null
                                            };
                                            _dbContext.RPOD_LR_DETAILS.Add(newLr);
                                            _dbContext.SaveChanges();
                                        }
                                        _dbContext.SaveChanges();
                                        transaction.Commit();
                                        transaction.Dispose();
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        transaction.Dispose();
                                    }
                                }
                            }
                        }

                    }
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    File.Delete(Path.Combine(path, fileName));
                    throw ex;
                }
            }

        }

        public async Task<bool> ConfirmReversePod()
        {
            string path = _appSetting.ReverseAttachmentsPath;
            string SharedFolderUserName = _appSetting.SharedFolderUserName;
            string SharedFolderPassword = _appSetting.SharedFolderPassword;
            string SharedFolderDomain = _appSetting.SharedFolderDomain;
            string fileName = "";
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var request = _httpContextAccessor.HttpContext.Request;
                    var payLoad = JsonConvert.DeserializeObject<ReversePodUpdation>(request.Form["Payload"].ToString());
                    var header = this._dbContext.RPOD_HEADER_DETAILS.FirstOrDefault(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID);
                    IFormFileCollection postedfiles = request.Form.Files;
                    if (postedfiles.Count > 0)
                    {
                        var ext = postedfiles[0].FileName.ToString().Split('.')[postedfiles[0].FileName.ToString().Split('.').Length - 1];
                        fileName = header.DC_NUMBER.ToString().Replace("/", "_") + "_" + header.CUSTOMER + "_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(":", "").Replace("/", "") + "." + ext;
                        using (Stream st = postedfiles[0].OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    ConvertedAttachmentProps convertedAttachment = _pdfCompresser.ConvertImagetoPdf(fileName, fileBytes);
                                    fileName = convertedAttachment.Filename;

                                    string fullPath = Path.Combine(path, convertedAttachment.Filename);
                                    try
                                    {
                                        using (var impersonationHelper = new ImpersonationHelper(SharedFolderUserName, SharedFolderDomain, SharedFolderPassword))
                                        {
                                            File.WriteAllBytes(Path.Combine(path, convertedAttachment.Filename), convertedAttachment.PDFcontent);
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

                                    RPOD_LR_ATTACHMENT lrDoc = new RPOD_LR_ATTACHMENT()
                                    {
                                        RPOD_HEADER_ID = payLoad.RPOD_HEADER_ID,
                                        Code = payLoad.Code,
                                        FileName = convertedAttachment.Filename,
                                        FilePath = fullPath,
                                        IsDeleted = false
                                    };

                                    if (payLoad.Code == 1)
                                    {
                                        header.STATUS = "In Transit";
                                    }
                                    else
                                    {
                                        header.STATUS = payLoad.STATUS;
                                    }
                                    var materials = _dbContext.RPOD_MATERIAL_DETAILS.Where(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID);
                                    foreach (var item in materials)
                                    {
                                        var material = payLoad.MATERIALS.Find(x => x.MATERIAL_ID == item.MATERIAL_ID);
                                        item.HAND_OVERED_QUANTITY = material.HAND_OVERED_QUANTITY;
                                        item.RECEIVED_QUANTITY = material.RECEIVED_QUANTITY;
                                        item.REMARKS = material.REMARKS;
                                        item.STATUS = material.STATUS;
                                    }

                                    _dbContext.RPOD_LR_ATTACHMENTS.Add(lrDoc);
                                    int attachStatus = 0;
                                    try
                                    {
                                        attachStatus = _dbContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("ReversePOD/ConfirmReversePOD/Exception:- " + ex.Message, ex);
                                    }
                                    if (attachStatus > 0)
                                    {
                                        var oldLr = _dbContext.RPOD_LR_DETAILS.Where(x => x.RPOD_HEADER_ID == payLoad.RPOD_HEADER_ID).OrderByDescending(x => x.LR_ID).ToList();
                                        if (oldLr.Count > 0 && oldLr[0].DC_DOC_ID == 0 && oldLr[0].DC_ACKNOWLEDGEMENT_DATE == null)
                                        {
                                            if (payLoad.Code == 1)
                                            {
                                                oldLr[0].CUSTOMER_DOC_ID = lrDoc.Id;
                                            }
                                            else
                                            {
                                                oldLr[0].DC_DOC_ID = lrDoc.Id;
                                                oldLr[0].DC_RECEIEVED_DATE = payLoad.DC_RECEIEVED_DATE;
                                                oldLr[0].DC_ACKNOWLEDGEMENT_DATE = DateTime.Now;
                                            }
                                        }
                                        else
                                        {
                                            var newLr = new RPOD_LR_DETAIL()
                                            {
                                                LR_NO = payLoad.LR_NO,
                                                LR_DATE = payLoad.LR_DATE,
                                                RPOD_HEADER_ID = payLoad.RPOD_HEADER_ID,
                                                CUSTOMER_DOC_ID = payLoad.Code == 1 ? lrDoc.Id : 0,
                                                DC_DOC_ID = payLoad.Code == 2 ? lrDoc.Id : 0,
                                                DC_RECEIEVED_DATE = payLoad.DC_RECEIEVED_DATE,
                                                DC_ACKNOWLEDGEMENT_DATE = payLoad.Code == 2 ? DateTime.Now : (DateTime?)null
                                            };
                                            _dbContext.RPOD_LR_DETAILS.Add(newLr);
                                            _dbContext.SaveChanges();
                                        }
                                        _dbContext.SaveChanges();
                                        transaction.Commit();
                                        transaction.Dispose();
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        transaction.Dispose();
                                    }

                                }
                            }
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    File.Delete(Path.Combine(path, fileName));
                    throw ex;
                }
            }
        }

        public async Task<bool> ReUploadReversePodLr(int id,string file)
        {
            try
            {
                var res = await _dbContext.RPOD_LR_ATTACHMENTS.FirstOrDefaultAsync(x => x.RPOD_HEADER_ID == id);
                if (res != null)
                {
                    res.FileName = file;
                    _dbContext.RPOD_LR_ATTACHMENTS.Update(res);
                    _dbContext.SaveChanges();
                }
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }



        #region UnUsed Functions

        private static readonly List<string> AcceptableClaimTypes = new List<string>
        {
            ClaimTypes.Name,
            ClaimTypes.Email,
            ClaimTypes.Role,
         };

        //PostMthod
        public async Task<InsertInvoiceResponse> InsertReversePOD(ReversePodDto reversePod)
        {
            InsertInvoiceResponse response;
            try
            {
                response = new InsertInvoiceResponse();

                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        if (!AcceptableClaimTypes.Contains(reversePod.CLAIM_TYPE))
                        {
                            response.Status = true;
                            response.StatusMessage = string.Format("Claim type is not acceptable - {0}", reversePod.CLAIM_TYPE);
                            return response;
                        }

                        RPOD_HEADER rpod = await _dbContext.RPOD_HEADER_DETAILS.FirstOrDefaultAsync(x => x.DC_NUMBER == reversePod.DC_NUMBER
                        && x.CUSTOMER == reversePod.CUSTOMER);

                        if (rpod == null)
                        {
                            RPOD_HEADER reversePOD = new RPOD_HEADER();
                            reversePOD.PLANT = reversePod.PLANT.ToString();
                            reversePOD.PLANT_NAME = reversePod.PLANT_NAME.ToString();
                            reversePOD.CUSTOMER = reversePod.CUSTOMER.ToString();
                            reversePOD.CUSTOMER_NAME = reversePod.CUSTOMER_NAME.ToString();
                            reversePOD.CLAIM_TYPE = reversePod.CLAIM_TYPE.ToString();
                            reversePOD.DC_NUMBER = reversePod.DC_NUMBER;
                            DateTime dcDate;
                            if (DateTime.TryParse(reversePod.DC_DATE, out dcDate))
                            {
                                reversePOD.DC_DATE = dcDate;
                            }
                            else
                            {
                                reversePOD.DC_DATE = (DateTime?)null;
                            }

                            DateTime slaDate;
                            if (DateTime.TryParse(reversePod.SLA_DATE, out slaDate))
                            {
                                reversePOD.SLA_DATE = slaDate;
                            }
                            else
                            {
                                reversePOD.SLA_DATE = (DateTime?)null;
                            }
                            reversePOD.PENDING_DAYS = reversePod.PENDING_DAYS;

                            reversePOD.TOTAL_QUANTITY = reversePod.TOTAL_QUANTITY.HasValue ? reversePod.TOTAL_QUANTITY : 0;
                            reversePOD.BILLED_QUANTITY = 0;
                            reversePOD.BALANCE_QUANTITY = reversePod.TOTAL_QUANTITY.HasValue ? reversePod.TOTAL_QUANTITY : 0;
                            reversePOD.STATUS = "Open";

                            await _dbContext.RPOD_HEADER_DETAILS.AddAsync(reversePOD);
                            int saveReversePod = 0;

                            try
                            {
                                saveReversePod = _dbContext.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }


                            if (saveReversePod > 0)
                            {
                                List<RPOD_MATERIAL> materials = new List<RPOD_MATERIAL>();

                                foreach (var item in reversePod.MATERIALS)
                                {
                                    RPOD_MATERIAL material = new RPOD_MATERIAL();
                                    material.RPOD_HEADER_ID = reversePOD.RPOD_HEADER_ID;
                                    material.MATERIAL_CODE = item.MATERIAL_CODE;
                                    material.QUANTITY = item.QUANTITY;
                                    material.REMARKS = "";
                                    material.HAND_OVERED_QUANTITY = 0;
                                    material.RECEIVED_QUANTITY = 0;
                                    material.STATUS = "Open";
                                    materials.Add(material);
                                }
                                await _dbContext.RPOD_MATERIAL_DETAILS.AddRangeAsync(materials);
                                int saveItems = 0;
                                try
                                {
                                    saveItems = _dbContext.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                                if (saveItems > 0)
                                {
                                    response.Status = true;
                                    response.StatusMessage = string.Format("DC Number {0} is inserted to Reverse POD", reversePOD.DC_NUMBER);
                                }
                                else
                                {
                                    _dbContext.RPOD_HEADER_DETAILS.Remove(reversePOD);
                                    _dbContext.SaveChanges();
                                }

                            }
                            else
                            {
                                response.Status = false;
                                response.StatusMessage = string.Format("Delivery Challan {0} is failed to insert into the database", reversePOD.DC_NUMBER);
                            }
                            transaction.Commit();
                            transaction.Dispose();
                            return response;
                        }
                        else
                        {
                            response.Status = false;
                            response.StatusMessage = string.Format("Delivery Challan {0} already exist in Reverse POD", rpod.DC_NUMBER);
                            transaction.Commit();
                            transaction.Dispose();
                            return response;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Status = false;
                        response.StatusMessage = ex.Message;
                        transaction.Rollback();
                        transaction.Dispose();
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //PostMethod
        public async Task<InsertInvoiceResponse> InsertWarrantyReplacement(List<WarrantyReplacementDto> warrantyReplacements)
        {
            InsertInvoiceResponse response;
            try
            {
                response = new InsertInvoiceResponse();
                foreach (var warrantyReplacement in warrantyReplacements)
                {
                    var rpod = await _dbContext.RPOD_HEADER_DETAILS.FirstOrDefaultAsync(x => x.DC_NUMBER == warrantyReplacement.DC_NUMBER);
                    if (rpod != null)
                    {
                        rpod.TOTAL_QUANTITY = warrantyReplacement.TOTAL_QUANTITY;
                        rpod.BILLED_QUANTITY = warrantyReplacement.BILLED_QUANTITY;
                        rpod.BALANCE_QUANTITY = warrantyReplacement.BALANCE_QUANTITY;

                        var materials = _dbContext.WARRANTY_REPLACEMENT.Where(x => x.RPOD_HEADER_ID == rpod.RPOD_HEADER_ID);
                        if (materials.Count() > 0)
                        {
                            foreach (var item in materials)
                            {
                                var mat = warrantyReplacement.MATERIALS.Find(m => m.MATERIAL_CODE == item.MATERIAL_CODE);
                                if (mat != null)
                                {
                                    item.TOTAL_QUANTITY = mat.TOTAL_QUANTITY;
                                    item.BILLED_QUANTITY = mat.BILLED_QUANTITY;
                                    item.BALANCE_QUANTITY = mat.BALANCE_QUANTITY;
                                    item.INV_NO = mat.INV_NO;
                                }
                                else
                                {
                                    WARRANTY_REPLACEMENT wr = new WARRANTY_REPLACEMENT()
                                    {
                                        RPOD_HEADER_ID = rpod.RPOD_HEADER_ID,
                                        INV_NO = item.INV_NO,
                                        MATERIAL_CODE = item.MATERIAL_CODE,
                                        TOTAL_QUANTITY = item.TOTAL_QUANTITY,
                                        BILLED_QUANTITY = item.BILLED_QUANTITY,
                                        BALANCE_QUANTITY = item.BALANCE_QUANTITY
                                    };
                                    await _dbContext.WARRANTY_REPLACEMENT.AddAsync(wr);
                                }

                            }
                        }
                        else
                        {
                            List<WARRANTY_REPLACEMENT> warrentyMaterials = new List<WARRANTY_REPLACEMENT>();
                            foreach (var item in warrantyReplacement.MATERIALS)
                            {
                                WARRANTY_REPLACEMENT wr = new WARRANTY_REPLACEMENT()
                                {
                                    RPOD_HEADER_ID = rpod.RPOD_HEADER_ID,
                                    INV_NO = item.INV_NO,
                                    MATERIAL_CODE = item.MATERIAL_CODE,
                                    TOTAL_QUANTITY = item.TOTAL_QUANTITY,
                                    BILLED_QUANTITY = item.BILLED_QUANTITY,
                                    BALANCE_QUANTITY = item.BALANCE_QUANTITY
                                };
                                warrentyMaterials.Add(wr);
                            }
                            await _dbContext.WARRANTY_REPLACEMENT.AddRangeAsync(warrentyMaterials);
                        }
                        _dbContext.SaveChangesAsync();
                    }
                }

                response.Status = true;
                response.StatusMessage = "Warranty replacement details has been updated successfully";
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //PostMethod
        public async Task<bool> UpdatePendingDays()
        {
            try
            {
                var res = _dbContext.RPOD_HEADER_DETAILS.Where(x => x.STATUS != "Confirmed");
                foreach (var item in res)
                {
                    if (item.SLA_DATE != null)
                    {
                        var diffOfDates = DateTime.Now - item.SLA_DATE;
                        item.PENDING_DAYS = diffOfDates.Value.Days;
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

        #endregion


    }
}
