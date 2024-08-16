using AREML.EPOD.Core.Dtos.Auth;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IAuthenticationRepository
    {
        Task<AuthenticationResponse> AuthenticateUser(LoginDetails loginDetails);
        Task<bool> ForgotPassword(ForgotPassword forgotPassword);
        Task<bool> ChangePassword(ForgotPasswordOTP forgotPasswordOTP);
    }
}
