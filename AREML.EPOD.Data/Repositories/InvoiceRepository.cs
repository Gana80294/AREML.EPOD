using AREML.EPOD.Core.Dtos.ReverseLogistics;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Data.Logging;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AuthContext _ctx;

        public InvoiceRepository(AuthContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetAllInvoiceDetails(Guid UserID)
        {
            try
            {
                var userResult = (from td in _ctx.Users
                                  select td).Where(x => x.UserID == UserID).FirstOrDefault();
                var result = (from td in _ctx.P_INV_HEADER_DETAILS
                              select td).Where(x => x.PLANT == "1" && x.IS_ACTIVE).ToList();
                return result;
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }

        public async Task<bool> DeleteInvoices(DeleteInvoiceDto payload)
        {
            try
            {
                var header = _ctx.P_INV_HEADER_DETAILS.FirstOrDefault(i => i.INV_NO == payload.InvNo);
                if (header == null)
                {
                    throw new Exception("Invoice not found");
                }

                if (header.STATUS.ToLower() == "open" || header.STATUS.ToLower().Contains("partially") || header.STATUS.ToLower() == "saved")
                {
                    var hId = header.HEADER_ID;
                    payload.log.TransID = hId;
                    header.IS_ACTIVE = false;
                    _ctx.SaveChanges();
                    _ctx.UserActionHistories.Add(payload.log);
                    _ctx.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception($"{header.STATUS} invoices cannot be deleted");                  
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<P_INV_HEADER_DETAIL>> GetInvoiceDetailByInvNo(string InvNo)
        {
            try
            {
                return this._ctx.P_INV_HEADER_DETAILS.Where(x => x.INV_NO == InvNo).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
    

