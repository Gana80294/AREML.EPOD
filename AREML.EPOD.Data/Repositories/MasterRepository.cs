using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class MasterRepository:IMasterRepository
    {
        private AuthContext _dbContext;

        public MasterRepository(AuthContext context)
        {
            this._dbContext = context;

        }
        public async Task<UserLoginHistory> LoginHistory(Guid UserID, string UserCode, string UserName)
        {
            try
            {
                UserLoginHistory loginData = new UserLoginHistory();
                loginData.UserID = UserID;
                loginData.UserCode = UserCode;
                loginData.UserName = UserName;
                loginData.LoginTime = DateTime.Now;
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
                _dbContext.UserLoginHistories.Add(loginData);
                await _dbContext.SaveChangesAsync();
                return loginData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
