using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class DashboardRepository:IDashboardRepository
    {
        private AuthContext _dbContext;

        public DashboardRepository(AuthContext context)
        {
            this._dbContext = context;

        }
    }
}
