using AREML.EPOD.Core.Entities.ForwardLogistics;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AREML.EPOD.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private IMasterRepository _masterRepository;
        public MasterController(IMasterRepository masterRepository)
        {
            _masterRepository = masterRepository;
        }

        #region User

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserWithRole userWithRole)
        {
            return Ok(await _masterRepository.CreateUser(userWithRole));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomers(List<CustomerData> customerDatas)
        {
            try
            {
                bool result = await _masterRepository.CreateCustomers(customerDatas);
                if (result)
                {
                    return Ok("Customers created successfully.");
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating customers.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetUserCreationErrorLog()
        {
            return Ok(_masterRepository.GetUserCreationErrorLog());
        }

        [HttpPost]
        public async Task<IActionResult> CreateBulkOrganization(List<OrganizationData> OrgDatas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _masterRepository.CreateBulkOrganization(OrgDatas);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBulkPlant(List<PlantData> PlntDatas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateBulkPlant(PlntDatas));

        }

        [HttpGet]
        public IActionResult GetSearchedUser(string key, int Page)
        {
            return Ok(_masterRepository.GetSearchedUser(key, Page));
        }

        [HttpPost]
        public async Task<IActionResult> DownloadUsersExcell(DownloadUserModel downloadUser)
        {
            var fileContent = await this._masterRepository.DownloadUsersExcell(downloadUser);

            if (fileContent == null || fileContent.Length == 0)
            {
                return NotFound("No data available for the requested filter.");
            }

            var fileName = $"Users_Excel_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserWithRole userWithRole)
        {
            return Ok(await _masterRepository.UpdateUser(userWithRole));
        }
        [HttpGet]
        public List<UserWithRole> GetAllUsers(int Page)
        {
            return _masterRepository.GetAllUsers(Page);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserWithRole userWithRole)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteUser(userWithRole));
        }

        #endregion

        #region Roles

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleWithApp roleWithApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateRole(roleWithApp));
        }

        [HttpGet]
        public List<RoleWithApp> GetAllRoles()
        {
            return (_masterRepository.GetAllRoles());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(RoleWithApp roleWithApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateRole(roleWithApp));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(RoleWithApp roleWithApp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteRole(roleWithApp));
        }

        #endregion

        #region Apps

        [HttpPost]
        public async Task<IActionResult> CreateApp(App App)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateApp(App));
        }

        [HttpGet]
        public List<App> GetAllApps()
        {
            return (_masterRepository.GetAllApps());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateApp(App App)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateApp(App));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteApp(App App)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteApp(App));
        }

        #endregion

        #region User-Manual

        [HttpPost]
        public async Task<IActionResult> AddUserManual()
        {
            var files = Request.Form.Files;
            return Ok(await _masterRepository.AddUserManual(files));
        }

        [HttpGet]
        [Route("GetUserManual")]
        public async Task<IActionResult> GetUserManual()
        {
            return Ok(await _masterRepository.GetUserManual());
        }
        #endregion

        #region Reasons

        [HttpPost]
        public async Task<IActionResult> CreateReason(Reason Reason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateReason(Reason));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBulkReason(List<ReasonData> reasonDatas)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateBulkReason(reasonDatas));
        }

        [HttpGet]
        public IActionResult GetAllReasons()
        {
            return Ok(_masterRepository.GetAllReasons());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReason(Reason Reason)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateReason(Reason));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReason(Reason Reason)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteReason(Reason));
        }

        #endregion

        #region Organizations

        [HttpPost]
        public async Task<IActionResult> CreateOrganization(Organization Organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateOrganization(Organization));
        }

        [HttpGet]
        public List<Organization> GetAllOrganizations()
        {
            return _masterRepository.GetAllOrganizations();
        }

        [HttpGet]
        public List<Organization> GetAllOrganizationsByUserID(Guid UserID)
        {
            return _masterRepository.GetAllOrganizationsByUserID(UserID);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrganization(Organization Organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateOrganization(Organization));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrganization(Organization Organization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteOrganization(Organization));
        }

        #endregion

        #region CustomerGroup

        [HttpPost]
        public async Task<IActionResult> CreateCustomerGroup(CustomerGroup userGroup)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateCustomerGroup(userGroup));
        }

        [HttpGet]
        public CustomerGroup GetSectorByCustomerGroup(string CustomerGroupCode)
        {
            return _masterRepository.GetSectorByCustomerGroup(CustomerGroupCode);
        }

        [HttpGet]
        public List<CustomerGroup> GetAllCustomerGroups()
        {
            return _masterRepository.GetAllCustomerGroups();
        }

        [HttpGet]
        public List<CustomerGroup> GetAllCustomerGroupsByUserID(Guid UserID)
        {
            return _masterRepository.GetAllCustomerGroupsByUserID(UserID);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCustomerGroup(CustomerGroup userGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateCustomerGroup(userGroup));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBulkCustomerGroup(List<CustomerGroupData> cgDatas)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateBulkCustomerGroup(cgDatas));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomerGroup(CustomerGroup userGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteCustomerGroup(userGroup));
        }

        #endregion

        #region ReversePOD

        [HttpGet]
        public List<UserWithRole> GetAllDCUsers()
        {
            return _masterRepository.GetAllDCUsers();
        }

        #endregion

        #region SalesGroup

        [HttpPost]
        public async Task<IActionResult> CreateSalesGroup(SLSCustGroupData slsGroup)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateSalesGroup(slsGroup));
        }

        [HttpGet]
        public List<SLSCustGroupData> GetAllSalesGroups()
        {
            return _masterRepository.GetAllSalesGroups();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalesGroup(SLSCustGroupData slsGroup)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdateSalesGroup(slsGroup));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBulkSalesGroup(List<SLSGroupWithCustomerGroupMap> sgDatas)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreateBulkSalesGroup(sgDatas));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSalesGroup(SLSCustGroupData sLsGroup)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeleteSalesGroup(sLsGroup));
        }

        #endregion

        #region TEST

        [HttpGet]
        public async Task<IActionResult> PushUserCreationLog()
        {
            return Ok(await _masterRepository.PushUserCreationLog());
        }

        #endregion

        #region Plants

        [HttpPost]
        public async Task<IActionResult> CreatePlant(PlantWithOrganization plantWithOrganization)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.CreatePlant(plantWithOrganization));
        }


        [HttpGet]
        public List<PlantWithOrganization> GetAllPlants()
        {
            return _masterRepository.GetAllPlants();
        }

        [HttpGet]
        public List<PlantWithOrganization> GetAllPlantsByUserID(Guid UserID)
        {
            return _masterRepository.GetAllPlantsByUserID(UserID);
        }

        [HttpGet]
        public List<PlantGroup> GetAllPlantGroups()
        {
            return _masterRepository.GetAllPlantGroups();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePlant(PlantWithOrganization plantWithOrganization)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.UpdatePlant(plantWithOrganization));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePlant(Plant Plant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.DeletePlant(Plant)); 
        }

        [HttpGet]
        public List<PlantOrganizationMap> GetAllPlantOrganizationMaps()
        {
            return _masterRepository.GetAllPlantOrganizationMaps();
        }

        #endregion

        #region grouping & download

        [HttpGet]
        public List<RolewithGroup> GetRoleandGroups()
        {
            return _masterRepository.GetRoleandGroups();
        }

        #endregion

        #region LogInAndChangePassword

        [HttpPost]
        public async Task<IActionResult> LoginHistory(Guid UserID, string UserCode, string UserName)
        {
            return Ok(await _masterRepository.LoginHistory(UserID, UserCode, UserName));    
        }

        [HttpGet]
        public List<UserLoginHistory> GetAllUsersLoginHistory()
        {
            return _masterRepository.GetAllUsersLoginHistory();
        }

        [HttpGet]
        public List<UserLoginHistory> GetCurrentUserLoginHistory(Guid UserID)
        {
            return _masterRepository.GetCurrentUserLoginHistory(UserID);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut(Guid UserID)
        {
            return Ok(await _masterRepository.SignOut(UserID));
        }

        #endregion

        #region ChangePassword

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.ChangePassword(changePassword));
        }

        [HttpPost]
        public async Task<IActionResult> SendResetLinkToMail(EmailModel emailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.SendResetLinkToMail(emailModel));
        }

        [HttpGet]
        [Route("SendOTPToMail")]
        public async Task<IActionResult> SendOTPToMail(string UserCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.SendOTPToMail(UserCode));
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository?.ForgotPassword(forgotPassword)); 

        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordUsingOTP(ForgotPasswordOTP forgotPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await _masterRepository.ChangePasswordUsingOTP(forgotPassword));
        }

        //[HttpPost]
        //public async Task<IActionResult> PasswordResetSendSMSOTP(resetPasswordOTPBody oTPBody)
        //{
        //    return Ok(await _masterRepository.ChangePasswordUsingOTP(oTPBody));
        //}

        [HttpPost]
        public async Task<IActionResult> ResetPasswordWithSMSOTP(AffrimativeOTPBody oTPBody)
        {
            return Ok(await _masterRepository.ResetPasswordWithSMSOTP(oTPBody));
        }

        #endregion


        #region Unlock User

        [HttpGet]
        public async Task<IActionResult> UnlockUser(Guid UserId)
        {
            return Ok(await _masterRepository.UnlockUser(UserId));
        }

        [HttpGet]
        public List<UserWithRole> GetAllLockedUsers()
        {
            return _masterRepository.GetAllLockedUsers();
        }


        #endregion
    }
}
