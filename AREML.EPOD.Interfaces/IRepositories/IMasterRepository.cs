using AREML.EPOD.Core.Entities.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IMasterRepository
    {
        Task<UserLoginHistory> LoginHistory(Guid UserID, string UserCode, string UserName);
    }
}
