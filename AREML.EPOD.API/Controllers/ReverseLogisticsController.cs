using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.ReverseLogistics;
using AREML.EPOD.Data.Logging;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReverseLogisticsController : ControllerBase
    {
        private IReverseLogisticsRepository _reverseLogisticsRepository;
        private AuthContext _ctx;
        private IConfiguration _configuration;
        public ReverseLogisticsController(IReverseLogisticsRepository reverseLogisticsRepository,AuthContext ctx, IConfiguration config)
        {
            _reverseLogisticsRepository = reverseLogisticsRepository;
            _ctx = ctx;
            _configuration = config;
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

        [HttpPost]
        public IActionResult InsertReversePOD(ReversePodDto reversePod)
        {
            InsertInvoiceResponse response;
            string[] claimTypes = new string[] { "ZA1N", "ZA2N", "ZA3N", "ZUSD", "ZGOD" };
            try
            {
                LogWriter.WriteProcessLog("***************************REVERSE POD INSERT PROCESS STARTED****************************");
                LogWriter.WriteProcessLog("ReversePOD/InsertReversePOD - Payload:- " + JsonConvert.SerializeObject(reversePod));
                response = new InsertInvoiceResponse();

                using (var transaction = _ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if (!claimTypes.Contains(reversePod.CLAIM_TYPE))
                        {
                            response.Status = true;
                            response.StatusMessage = string.Format("Claim type is not acceptable - {0}", reversePod.CLAIM_TYPE);
                            return Ok(response);
                        }

                        RPOD_HEADER rpod = _ctx.RPOD_HEADER_DETAILS.FirstOrDefault(x => x.DC_NUMBER == reversePod.DC_NUMBER
                        && x.CUSTOMER == reversePod.CUSTOMER);

                        if (rpod == null)
                        {
                            LogWriter.WriteProcessLog("***************************REVERSE POD INSERT HEADER DATA PROCESS STARTED****************************");
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

                            _ctx.RPOD_HEADER_DETAILS.Add(reversePOD);
                            int saveReversePod = 0;

                            try
                            {
                                saveReversePod = _ctx.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                LogWriter.WriteProcessLog("ReversePOD/InsertReversePOD/HeaderSaveChanges/Exception:- " + ex.Message, ex);
                            }


                            if (saveReversePod > 0)
                            {
                                LogWriter.WriteProcessLog("***************************REVERSE POD INSERT ITEM PROCESS STARTED****************************");

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
                                _ctx.RPOD_MATERIAL_DETAILS.AddRange(materials);
                                int saveItems = 0;
                                try
                                {
                                    saveItems = _ctx.SaveChanges();
                                }
                                catch (Exception ex)
                                {
                                    LogWriter.WriteProcessLog("ReversePOD/InsertReversePOD/ItemsSaveChanges/Exception:- " + ex.Message, ex);
                                }

                                if (saveItems > 0)
                                {
                                    LogWriter.WriteProcessLog("PODConfirmation/InsertReversePOD:- " + string.Format("Invoice No {0} with Invoice Type {1}  is inserted ", reversePod.INV_NO, reversePod.CUSTOMER));
                                    response.Status = true;
                                    response.StatusMessage = string.Format("DC Number {0} is inserted to Reverse POD", reversePOD.DC_NUMBER);
                                }
                                else
                                {
                                    _ctx.RPOD_HEADER_DETAILS.Remove(reversePOD);
                                    _ctx.SaveChanges();
                                }

                            }
                            else
                            {
                                response.Status = false;
                                response.StatusMessage = string.Format("Delivery Challan {0} is failed to insert into the database", reversePOD.DC_NUMBER);
                            }
                            transaction.Commit();
                            transaction.Dispose();
                            LogWriter.WriteProcessLog("***************************REVERSE POD INSERT PROCESS ENDED****************************");
                            return Ok(response);
                        }
                        else
                        {
                            LogWriter.WriteProcessLog("PODConfirmation/InsertReversePOD/Exception:- " + string.Format("DC Number {0} already exist", rpod.DC_NUMBER));
                            response.Status = false;
                            response.StatusMessage = string.Format("Delivery Challan {0} already exist in Reverse POD", rpod.DC_NUMBER);
                            transaction.Commit();
                            transaction.Dispose();
                            return Ok(response);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.WriteProcessLog("ReverseLogistics/InsertReversePOD/Exception:- " + ex.Message, ex);
                        response.Status = false;
                        response.StatusMessage = ex.Message;
                        transaction.Rollback();
                        transaction.Dispose();
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
