using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Interfaces.IRepositories
{
    public interface IMasterRepository
    {
        #region User
        Task<bool> CreateUser(UserWithRole userWithRole);
        Task<bool> CreateCustomers(List<CustomerData> customerDatas);
        List<UserCreationErrorLog> GetUserCreationErrorLog();
        Task CreateBulkOrganization(List<OrganizationData> OrgDatas);
        Task<bool> CreateBulkPlant(List<PlantData> PlntDatas);
        List<UserWithRole> GetSearchedUser(string key, int Page);
        bool DownloadUsersExcell(DownloadUserModel downloadUser);
        Task<bool> UpdateUser(UserWithRole userWithRole);
        List<UserWithRole> GetAllUsers(int Page);
        Task<bool> DeleteUser(UserWithRole userWithRole);
        #endregion

        #region Roles
        Task<bool> CreateRole(RoleWithApp roleWithApp);
        List<RoleWithApp> GetAllRoles();
        Task<bool> UpdateRole(RoleWithApp roleWithApp);
        Task<bool> DeleteRole(RoleWithApp roleWithApp);
        #endregion

        #region Apps
        Task<App> CreateApp(App App);
        List<App> GetAllApps();
        Task<App> UpdateApp(App App);
        Task<App> DeleteApp(App App);
        #endregion

        #region User-Manual
        Task<bool> AddUserManual(IFormFileCollection files);
        Task<List<UserManualDocStore>> GetUserManual();
        #endregion

        #region Reasons
        Task<Reason> CreateReason(Reason Reason);
        Task<List<Reason>> CreateBulkReason(List<ReasonData> reasonDatas);
        List<Reason> GetAllReasons();
        Task<Reason> UpdateReason(Reason Reason);
        Task<Reason> DeleteReason(Reason Reason);

        #endregion

        #region Organizations

        Task<Organization> CreateOrganization(Organization Organization);
        List<Organization> GetAllOrganizations();
        List<Organization> GetAllOrganizationsByUserID(Guid UserID);
        Task<Organization> UpdateOrganization(Organization Organization);
        Task<Organization> DeleteOrganization(Organization Organization);
        #endregion

        #region CustomerGroup

        Task<CustomerGroup> CreateCustomerGroup(CustomerGroup userGroup);
        CustomerGroup GetSectorByCustomerGroup(string CustomerGroupCode);
        List<CustomerGroup> GetAllCustomerGroups();
        List<CustomerGroup> GetAllCustomerGroupsByUserID(Guid UserID);
        Task<CustomerGroup> UpdateCustomerGroup(CustomerGroup userGroup);
        Task<bool> CreateBulkCustomerGroup(List<CustomerGroupData> cgDatas);
        Task<CustomerGroup> DeleteCustomerGroup(CustomerGroup userGroup);

        #endregion

        #region ReversePOD
        List<UserWithRole> GetAllDCUsers();

        #endregion

        #region SalesGroup

        Task<bool> CreateSalesGroup(SLSCustGroupData slsGroup);
        List<SLSCustGroupData> GetAllSalesGroups();
        Task<bool> UpdateSalesGroup(SLSCustGroupData slsGroup);
        Task<List<SLSGroupWithCustomerGroupMap>> CreateBulkSalesGroup(List<SLSGroupWithCustomerGroupMap> sgDatas);
        Task<List<SLSGroupWithCustomerGroupMap>> DeleteSalesGroup(SLSCustGroupData sLsGroup);


        #endregion

        #region TEST

        Task<bool> PushUserCreationLog();

        #endregion

        #region Plants
        Task<bool> CreatePlant(PlantWithOrganization plantWithOrganization);
        List<PlantWithOrganization> GetAllPlants();
        List<PlantWithOrganization> GetAllPlantsByUserID(Guid UserID);
        List<PlantGroup> GetAllPlantGroups();
        Task<bool> UpdatePlant(PlantWithOrganization plantWithOrganization);
        Task<Plant> DeletePlant(Plant Plant);
        List<PlantOrganizationMap> GetAllPlantOrganizationMaps();

        #endregion

        #region grouping & download

        List<RolewithGroup> GetRoleandGroups();

        #endregion

        #region LogInAndChangePassword
        Task<UserLoginHistory> LoginHistory(Guid UserID, string UserCode, string UserName);
        List<UserLoginHistory> GetAllUsersLoginHistory();
        List<UserLoginHistory> GetCurrentUserLoginHistory(Guid UserID);
        Task<UserLoginHistory> SignOut(Guid UserID);

        #endregion

        #region ChangePassword
        Task<User> ChangePassword(ChangePassword changePassword);
        Task<bool> SendResetLinkToMail(EmailModel emailModel);
        Task<User> SendOTPToMail(string UserCode);
        Task<bool> ForgotPassword(ForgotPassword forgotPassword);
        Task<bool> ChangePasswordUsingOTP(ForgotPasswordOTP forgotPassword);
        // Task<IActionResult> PasswordResetSendSMSOTP(resetPasswordOTPBody oTPBody);
        Task<IActionResult> ResetPasswordWithSMSOTP(AffrimativeOTPBody oTPBody);
        #endregion

        #region Unlock User

        Task<bool> UnlockUser(Guid UserId);
        List<UserWithRole> GetAllLockedUsers();
        #endregion
    }
}
