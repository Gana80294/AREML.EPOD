using AREML.EPOD.Core.Entities;
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
    public class ReportRepository:IReportRepository
    {
        private AuthContext _dbContext;

        public ReportRepository(AuthContext context)
        {
            this._dbContext = context;
        }
        public async Task<List<string>> GetDivisions()
        {
            try
            {
                var result = await (from tb in _dbContext.P_INV_HEADER_DETAIL
                              where tb.IS_ACTIVE
                              select tb.DIVISION).Distinct().ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
