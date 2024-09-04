using AREML.EPOD.Core.Dtos.Auth;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities.Master;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IAuthenticationRepository
    {
        Task<string> AuthenticateUser(LoginDetails loginDetails);
        Task<bool> ForgotPassword(ForgotPassword forgotPassword);
        Task<string> ChangePassword(ChangePassword changePassword);
        Task<OTPResponseBody> PasswordResetSendSMSOTP(string username);
        Task<string> ResetPasswordWithSMSOTP(AffrimativeOTPBody otpBody);
        Task<AuthenticationResponse> AuthenticateMobileUser(LoginDetails loginDetails);
    }
}
