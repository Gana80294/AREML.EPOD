using AREML.EPOD.Core.Dtos.Auth;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private IAuthenticationRepository _auth;
        public AuthenticationController(IAuthenticationRepository auth)
        {
            this._auth = auth;
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticateUser(LoginDetails loginDetails)
        {
            return Ok(await _auth.AuthenticateUser(loginDetails));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            try
            {
                return Ok(await this._auth.ForgotPassword(forgotPassword));
            }
            catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            var result = await this._auth.ChangePassword(changePassword);
            return Ok(new {message = result});
        }

        [HttpPost]
        public async Task<IActionResult> PasswordResetSendSMSOTP(string username)
        {
            return Ok(await this._auth.PasswordResetSendSMSOTP(username));
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordWithSMSOTP(AffrimativeOTPBody otpBody)
        {
            var result = await this._auth.ResetPasswordWithSMSOTP(otpBody);
            return Ok(new { message = result });
        }
    }
}
