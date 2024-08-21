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
            return Ok(await this._auth.ForgotPassword(forgotPassword));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ForgotPasswordOTP forgotPassword)
        {
            return Ok(await this._auth.ChangePassword(forgotPassword));
        }

    }
}
