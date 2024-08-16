using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Mappings;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Data.Logging;
using AREML.EPOD.Interfaces.IRepositories;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace AREML.EPOD.Data.Repositories
{
    public class MasterRepository : IMasterRepository
    {
        private readonly AuthContext _ctx;
        private readonly IConfiguration _configuration;
        private EmailHelper _emailHelper;
        private readonly ExcelHelper _excelHelper;
        private readonly PasswordEncryptor _passwordEncryptor;
        private IMapper _mapper;
        private int _tokenTimespan = 0;

        public MasterRepository(AuthContext ctx, IConfiguration configuration, PasswordEncryptor passwordEncryptor,IMapper mapper)
        {
            _ctx = ctx;
            _configuration = configuration;
            _passwordEncryptor = passwordEncryptor;
            _mapper = mapper;
        }


        #region User

        public async Task<bool> CreateUser(UserWithRole userWithRole)
        {
            try
            {
                // Creating User
                User user1 = (from tb1 in _ctx.Users
                              where tb1.UserCode == userWithRole.UserCode && tb1.IsActive == true
                              select tb1).FirstOrDefault();

                if (user1 == null)
                {
                    string DefaultPassword = _configuration["AppSettings:DefaultPassword"];
                    var user = _mapper.Map<User>(userWithRole);
                    user.UserID = Guid.NewGuid();
                    user.Password = _passwordEncryptor.Encrypt(userWithRole.Password, true);
                    user.CreatedBy=userWithRole.CreatedBy;
                    user.IsActive = true;
                    user.IsLocked = false;
                    var result=_ctx.Users.Add(user);
                    await _ctx.SaveChangesAsync();
                    var creationErrorLog = _mapper.Map<UserCreationErrorLog>(userWithRole);
                    creationErrorLog.LogReson = "User created Successfully ";
                    creationErrorLog.RoleName = _ctx.Roles.FirstOrDefault(p => p.RoleID == userWithRole.RoleID).RoleName;
                    _ctx.UserCreationErrorLogs.Add(creationErrorLog);
                    await _ctx.SaveChangesAsync();
                    var userRole = _mapper.Map<UserRoleMap>(userWithRole);
                    userRole.UserID = user.UserID;
                    var r = _ctx.UserRoleMaps.Add(userRole);
                    await _ctx.SaveChangesAsync();

                    if (userWithRole.OrganizationList != null)
                    {
                        foreach (var org in userWithRole.OrganizationList)
                        {
                            UserOrganizationMap userOrganizationMap = new UserOrganizationMap()
                            {
                                UserID = user.UserID,
                                OrganizationCode = org,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            var r1 = _ctx.UserOrganizationMaps.Add(userOrganizationMap);
                        }
                    }
                    await _ctx.SaveChangesAsync();

                    if (userWithRole.PlantList != null)
                    {
                        foreach (var PlantID in userWithRole.PlantList)
                        {
                            UserPlantMap userPlantMap = new UserPlantMap()
                            {
                                UserID = user.UserID,
                                PlantCode = PlantID,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            var r1 = _ctx.UserPlantMaps.Add(userPlantMap);
                        }
                    }
                    await _ctx.SaveChangesAsync();

                    if (userWithRole.SLSgroups != null || userWithRole.SLSgroups.Count > 0)
                    {
                        foreach (var sls in userWithRole.SLSgroups)
                        {
                            UserSalesGroupMap salesGroupMap = new UserSalesGroupMap()
                            {
                                CreatedOn = DateTime.Now,
                                UserID = user.UserID,
                                SGID = sls,
                                IsActive = true,

                            };
                            _ctx.UserSalesGroupMaps.Add(salesGroupMap);
                        }
                    }
                    await _ctx.SaveChangesAsync();
                    //Send Mail
                    try
                    {
                        new Thread(async () =>
                        {
                            await _emailHelper.SendMailToUser(user.Email, user.UserName, userWithRole.Password);
                        }).Start();
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
                else
                {
                    throw new Exception("User with same user code already exist");
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<bool> CreateCustomers(List<CustomerData> customerDatas)
        {
            _ctx.Database.SetCommandTimeout(300);
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {
                    var CustomerRoleID = _ctx.Roles.Where(x => x.RoleName.ToLower() == "customer").Select(y => y.RoleID).FirstOrDefault();
                    if (CustomerRoleID != null)
                    {
                        foreach (CustomerData userData in customerDatas)
                        {
                            LogWriter.WriteToFile($"Master/CreateCustomers : Trying to insert Customer {userData.UserCode}");
                            if (!string.IsNullOrEmpty(userData.UserCode) && !string.IsNullOrEmpty(userData.Email) && !string.IsNullOrEmpty(userData.Password))
                            {
                                User user1 = (from tb1 in _ctx.Users
                                              where tb1.UserCode == userData.UserCode && tb1.IsActive == true
                                              select tb1).FirstOrDefault();
                                if (user1 == null)
                                {
                                    User user2 = (from tb1 in _ctx.Users
                                                  where tb1.Email == userData.Email && tb1.IsActive == true
                                                  select tb1).FirstOrDefault();
                                    if (user2 == null)
                                    {
                                        var user = _mapper.Map<User>(userData);
                                        user.UserID = Guid.NewGuid();
                                        user.Password = _passwordEncryptor.Encrypt(userData.Password, true);
                                        user.CreatedBy = "Admin";
                                        user.IsActive = true;
                                        user.IsLocked = false;
                                        var result = _ctx.Users.Add(user);
                                        await _ctx.SaveChangesAsync();

                                        UserRoleMap UserRole = new UserRoleMap()
                                        {
                                            RoleID = CustomerRoleID,
                                            UserID = user.UserID,
                                            IsActive = true,
                                            CreatedBy = "Admin",
                                            CreatedOn = DateTime.Now
                                        };

                                        var r = _ctx.UserRoleMaps.Add(UserRole);
                                        var errorLog = _mapper.Map<UserCreationErrorLog>(userData); 
                                        errorLog.RoleName = "Customer";
                                        errorLog.LogReson = $"Customer {userData.UserCode} inserted successfully";
                                        _ctx.UserCreationErrorLogs.Add(errorLog);

                                        await _ctx.SaveChangesAsync();
                                        LogWriter.WriteToFile($"Master/CreateCustomers : Customer {userData.UserCode} inserted successfully");
                                    }
                                    else
                                    {
                                        var errorLog = _mapper.Map<UserCreationErrorLog>(userData);                                       
                                        errorLog.RoleName = "Customer";
                                        errorLog.LogReson = $"Customer already exist";
                                        _ctx.UserCreationErrorLogs.Add(errorLog);
                                        await _ctx.SaveChangesAsync();
                                        LogWriter.WriteToFile($"Master/CreateCustomers : Customer with same email address {userData.Email} already exist");
                                    }
                                }
                                else
                                {
                                    var errorLog = _mapper.Map<UserCreationErrorLog>(userData);
                                    errorLog.RoleName = "Customer";
                                    errorLog.LogReson = $"Customer already exist";
                                    _ctx.UserCreationErrorLogs.Add(errorLog);
                                    await _ctx.SaveChangesAsync();
                                    LogWriter.WriteToFile($"Master/CreateCustomers : Customer with same user code {userData.UserCode} already exist");
                                }
                            }
                        }

                    }
                    else
                    {
                        transaction.Commit();
                        transaction.Dispose();
                        throw new Exception("Customer Role ID does not exist");
                    }
                    transaction.Commit();
                    transaction.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    throw ex;
                }

            }
        }

        public List<UserCreationErrorLog> GetUserCreationErrorLog()
        {
            try
            {
                return _ctx.UserCreationErrorLogs.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task CreateBulkOrganization(List<OrganizationData> OrgDatas)
        {
            try
            {
                foreach (var org in OrgDatas)
                {
                    LogWriter.WriteToFile("Master/CreateBulkOrganization:- " + "Trying to insert " + org.organizationCode);
                    Organization Organization1 = (from tb in _ctx.Organizations
                                                  where tb.IsActive && tb.OrganizationCode == org.organizationCode
                                                  select tb).FirstOrDefault();
                    if (Organization1 == null)
                    {
                        Organization organization = new Organization();
                        organization.CreatedOn = DateTime.Now;
                        organization.IsActive = true;
                        organization.CreatedBy = "Admin";
                        organization.Description = org.organizationDescription;
                        organization.ModifiedBy = "Admin";
                        organization.ModifiedOn = DateTime.Now;
                        organization.OrganizationCode = org.organizationCode;
                        var result = _ctx.Organizations.Add(organization);
                        await _ctx.SaveChangesAsync();
                        LogWriter.WriteToFile("Master/CreateBulkOrganization:- " + org.organizationCode + "inserted successfully");
                    }
                    else
                    {
                        LogWriter.WriteToFile("Master/CreateBulkOrganization:- " + org.organizationCode + " already exists");
                    }
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> CreateBulkPlant(List<PlantData> PlntDatas)
        {
            try
            {
                foreach (var plnt in PlntDatas)
                {
                    Organization torg = (from tb in _ctx.Organizations where tb.IsActive && tb.OrganizationCode == plnt.organization select tb).FirstOrDefault();
                    if (torg != null)
                    {
                        Plant Plant1 = (from tb in _ctx.Plants
                                        where tb.IsActive && tb.PlantCode == plnt.plantCode
                                        select tb).FirstOrDefault();
                        if (Plant1 == null)
                        {
                            Plant Plant = new Plant();
                            Plant.PlantCode = plnt.plantCode;
                            Plant.Description = plnt.plantDescription;
                            Plant.CreatedBy = "Admin";
                            Plant.CreatedOn = DateTime.Now;
                            Plant.IsActive = true;
                            var result = _ctx.Plants.Add(Plant);
                            await _ctx.SaveChangesAsync();
                            if (plnt.organization != null)
                            {
                                PlantOrganizationMap plantOrganizationMap = new PlantOrganizationMap()
                                {
                                    PlantCode = Plant.PlantCode,
                                    OrganizationCode = plnt.organization,
                                    IsActive = true,
                                    CreatedOn = DateTime.Now
                                };
                                var r1 = _ctx.PlantOrganizationMaps.Add(plantOrganizationMap);
                            }
                            await _ctx.SaveChangesAsync();
                        }
                    }
                }
                return true;


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserWithRole> GetSearchedUser(string key, int Page)
        {
            try
            {


                var noRecords = 20;
                var SkipValue = (Page - 1) * noRecords;
                var TakeValue = noRecords;
                var result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              where tb.IsActive && tb1.IsActive == true && (tb.UserCode == key || tb.Email == key || tb.ContactNumber == key || tb.UserName.Contains(key))
                              orderby tb.CreatedOn
                              select new
                              {
                                  tb.UserID,
                                  tb.UserCode,
                                  tb.UserName,
                                  tb.Email,
                                  tb.ContactNumber,
                                  tb.Password,
                                  tb.IsActive,
                                  tb.CreatedOn,
                                  tb.ModifiedOn,
                                  tb1.RoleID,
                                  tb.CustomerGroupCode
                              }).Skip(SkipValue).Take(noRecords).ToList();

                List<UserWithRole> UserWithRoleList = new List<UserWithRole>();

                result.ForEach(record =>
                {
                    UserWithRoleList.Add(new UserWithRole()
                    {
                        UserID = record.UserID,
                        UserCode = record.UserCode,
                        UserName = record.UserName,
                        OrganizationList = (from tb in _ctx.UserOrganizationMaps
                                            join tb1 in _ctx.Organizations on tb.OrganizationCode equals tb1.OrganizationCode
                                            where tb.UserID == record.UserID
                                            select tb.OrganizationCode).ToList(),
                        PlantList = (from tb in _ctx.UserPlantMaps
                                     join tb1 in _ctx.Plants on tb.PlantCode equals tb1.PlantCode
                                     where tb.UserID == record.UserID
                                     select tb.PlantCode).ToList(),
                        Email = record.Email,
                        ContactNumber = record.ContactNumber,
                        Password = _passwordEncryptor.Decrypt(record.Password, true),
                        IsActive = record.IsActive,
                        CreatedOn = record.CreatedOn,
                        ModifiedOn = record.ModifiedOn,
                        RoleID = record.RoleID,
                        CustomerGroup = record.CustomerGroupCode,
                        SLSgroups = _ctx.UserSalesGroupMaps.Where(k => k.UserID == record.UserID).Select(k => k.SGID).ToList()

                    });

                });
                return UserWithRoleList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DownloadUsersExcell(DownloadUserModel downloadUser)
        {
            try
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                bool isSlsgroup = downloadUser.isAmUser;
                List<ExportUserModel> result;
                if (isSlsgroup && downloadUser.SGID.Length > 0)
                {
                    result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID
                              join tb3 in _ctx.UserSalesGroupMaps on tb.UserID equals tb3.UserID
                              where tb2.RoleID == downloadUser.Role && (downloadUser.SGID.Any(t => t == tb3.SGID) || !isSlsgroup) && tb.IsActive
                              select new ExportUserModel()
                              {
                                  UserID = tb.UserID,
                                  UserCode = tb.UserCode,
                                  UserName = tb.UserName,
                                  EmailID = tb.Email,
                                  Mobile = tb.ContactNumber,
                                  RoleName = tb2.RoleName,
                                  isAmuser = tb2.RoleName.ToLower() == "amararaja user" ? true : false

                              }).ToList();
                }
                else if (isSlsgroup && downloadUser.SGID.Length == 0)
                {
                    LogWriter.WriteToFile("No SLs count");
                    result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID

                              where tb2.RoleID == downloadUser.Role && tb.IsActive
                              select new ExportUserModel()
                              {
                                  UserID = tb.UserID,
                                  UserCode = tb.UserCode,
                                  UserName = tb.UserName,
                                  EmailID = tb.Email,
                                  Mobile = tb.ContactNumber,
                                  RoleName = tb2.RoleName,
                                  isAmuser = tb2.RoleName.ToLower() == "amararaja user" ? true : false

                              }).ToList();
                }
                else
                {
                    result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID

                              where tb2.RoleID == downloadUser.Role && tb.IsActive
                              select new ExportUserModel()
                              {
                                  UserID = tb.UserID,
                                  UserCode = tb.UserCode,
                                  UserName = tb.UserName,
                                  EmailID = tb.Email,
                                  Mobile = tb.ContactNumber,
                                  RoleName = tb2.RoleName,
                                  isAmuser = tb2.RoleName.ToLower() == "amararaja user" ? true : false

                              }).ToList();
                }
                List<ExportUserModel> overAllusers = new List<ExportUserModel>();
                LogWriter.WriteToFile(result.Count.ToString());
                if (result.Count > 0)
                {
                    result.ForEach(k =>
                    {
                        if (k.isAmuser)
                        {
                            var orgs = (from tb in _ctx.UserOrganizationMaps join tb1 in _ctx.Users on tb.UserID equals tb1.UserID where tb1.IsActive && tb.IsActive && tb1.UserID == k.UserID select tb.OrganizationCode).Distinct().ToList();
                            var plts = (from tb in _ctx.UserPlantMaps join tb1 in _ctx.Users on tb.UserID equals tb1.UserID where tb1.IsActive && tb.IsActive && tb1.UserID == k.UserID select tb.PlantCode).Distinct().ToList();


                            foreach (string o in orgs)
                            {
                                //   k.Organization = o;
                                foreach (string p in plts)
                                {
                                    List<string> sls = null;
                                    if (downloadUser.SGID.Count() > 0)
                                    {
                                        sls = (from tb in _ctx.SLSGroupWithCustomerGroupMaps join tb1 in _ctx.UserSalesGroupMaps on tb.SGID equals tb1.SGID where tb1.UserID == k.UserID && downloadUser.SGID.Any(t => t == tb1.SGID) select tb.SLSGroupCode).Distinct().ToList();

                                    }
                                    else
                                    {
                                        sls = (from tb in _ctx.SLSGroupWithCustomerGroupMaps join tb1 in _ctx.UserSalesGroupMaps on tb.SGID equals tb1.SGID where tb1.UserID == k.UserID select tb.SLSGroupCode).Distinct().ToList();
                                    }
                                    //  k.Plant = p;
                                    if (sls.Count > 0)
                                    {
                                        foreach (string s in sls)
                                        {
                                            //  k.SalesGroups = s;
                                            ExportUserModel um = new ExportUserModel();
                                            um.UserID = k.UserID;
                                            um.UserName = k.UserName;
                                            um.UserCode = k.UserCode;
                                            um.Mobile = k.Mobile;
                                            um.EmailID = k.EmailID;
                                            um.RoleName = k.RoleName;
                                            um.Plant = p;
                                            um.Organization = o;
                                            um.isAmuser = k.isAmuser;
                                            um.SalesGroups = s;
                                            overAllusers.Add(um);
                                        }
                                    }
                                    else
                                    {
                                        LogWriter.WriteToFile("NO SlsGroup");
                                        ExportUserModel um = new ExportUserModel();
                                        um.UserID = k.UserID;
                                        um.UserName = k.UserName;
                                        um.UserCode = k.UserCode;
                                        um.Mobile = k.Mobile;
                                        um.EmailID = k.EmailID;
                                        um.RoleName = k.RoleName;
                                        um.Plant = p;
                                        um.Organization = o;
                                        um.isAmuser = k.isAmuser;
                                        um.SalesGroups = "";
                                        overAllusers.Add(um);
                                    }
                                }
                            }

                        }
                        else
                        {
                            overAllusers.Add(k);
                        }

                    });


                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = _excelHelper.CreateNPOIExportUserworksheet(overAllusers, workbook);
                    DateTime dt1 = DateTime.Today;
                    string dtstr1 = dt1.ToString("ddMMyyyyHHmmss");
                    var FileNm = $"User details_{dtstr1}.xlsx";

                    MemoryStream memory = new MemoryStream();


                    workbook.Write(memory);

                    byte[] fileByteArray = memory.ToArray();


                    var statuscode = HttpStatusCode.OK;
                    //response = Request.CreateResponse(statuscode);
                    response.Content = new StreamContent(new MemoryStream(fileByteArray));
                    response.Content.Headers.Add("x-filename", FileNm);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    //response.Content.Headers.ContentLength = contentLength;
                    ContentDispositionHeaderValue contentDisposition = null;

                    if (ContentDispositionHeaderValue.TryParse("inline; filename=" + FileNm, out contentDisposition))
                    {
                        response.Content.Headers.ContentDisposition = contentDisposition;
                    }

                    return true;
                }
                else
                {                    
                    throw new Exception("No user has been assigned to this sales group");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateUser(UserWithRole userWithRole)
        {
            try
            {
                User user1 = (from tb1 in _ctx.Users
                              where tb1.IsActive == true && tb1.UserID == userWithRole.UserID
                              select tb1).FirstOrDefault();

                if (user1 != null)
                {
                    //Updating User details
                    var user = (from tb in _ctx.Users
                                where tb.IsActive &&
                                tb.UserID == userWithRole.UserID
                                select tb).FirstOrDefault();
                    user.UserName = userWithRole.UserName;
                    user.Email = userWithRole.Email;
                    //user.Password = Encrypt(userWithRole.Password, true);
                    user.ContactNumber = userWithRole.ContactNumber;
                    user.IsActive = true;
                    user.ModifiedOn = DateTime.Now;
                    user.ModifiedBy = userWithRole.ModifiedBy;

                    await _ctx.SaveChangesAsync();

                    UserRoleMap OldUserRole = _ctx.UserRoleMaps.Where(x => x.UserID == userWithRole.UserID && x.IsActive == true).FirstOrDefault();
                    if (OldUserRole.RoleID != userWithRole.RoleID)
                    {
                        //Delete old role related to the user
                        _ctx.UserRoleMaps.Remove(OldUserRole);
                        await _ctx.SaveChangesAsync();

                        //Add new roles for the user
                        UserRoleMap UserRole = new UserRoleMap()
                        {
                            RoleID = userWithRole.RoleID,
                            UserID = user.UserID,
                            IsActive = true,
                            CreatedBy = userWithRole.ModifiedBy,
                            CreatedOn = DateTime.Now,
                        };
                        var r = _ctx.UserRoleMaps.Add(UserRole);
                        await _ctx.SaveChangesAsync();
                    }

                    _ctx.UserOrganizationMaps.Where(x => x.UserID == userWithRole.UserID).ToList().ForEach(y => _ctx.UserOrganizationMaps.Remove(y));
                    await _ctx.SaveChangesAsync();
                    if (userWithRole.OrganizationList != null)
                    {
                        foreach (var org in userWithRole.OrganizationList)
                        {
                            UserOrganizationMap userOrganizationMap = new UserOrganizationMap()
                            {
                                UserID = user.UserID,
                                OrganizationCode = org,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            var r1 = _ctx.UserOrganizationMaps.Add(userOrganizationMap);
                        }
                    }
                    await _ctx.SaveChangesAsync();

                    _ctx.UserPlantMaps.Where(x => x.UserID == userWithRole.UserID).ToList().ForEach(y => _ctx.UserPlantMaps.Remove(y));
                    await _ctx.SaveChangesAsync();
                    if (userWithRole.PlantList != null)
                    {
                        foreach (var PlantID in userWithRole.PlantList)
                        {
                            UserPlantMap userPlantMap = new UserPlantMap()
                            {
                                UserID = user.UserID,
                                PlantCode = PlantID,
                                IsActive = true,
                                CreatedOn = DateTime.Now
                            };
                            var r1 = _ctx.UserPlantMaps.Add(userPlantMap);
                        }
                    }
                    await _ctx.SaveChangesAsync();
                    _ctx.UserSalesGroupMaps.RemoveRange(_ctx.UserSalesGroupMaps.Where(k => k.UserID == userWithRole.UserID));
                    await _ctx.SaveChangesAsync();
                    if (userWithRole.SLSgroups != null || userWithRole.SLSgroups.Count > 0)
                    {
                        foreach (var sls in userWithRole.SLSgroups)
                        {
                            UserSalesGroupMap userSales = new UserSalesGroupMap()
                            {
                                UserID = userWithRole.UserID,
                                SGID = sls,
                                CreatedOn = DateTime.Now
                            };
                            _ctx.UserSalesGroupMaps.Add(userSales);
                        }
                    }
                    await _ctx.SaveChangesAsync();
                    return true;

                }
                else
                {
                    // return HttpContent(HttpStatusCode.BadRequest, "User with same user code already exist");
                    throw new Exception("User with same user code already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> DeleteUser(UserWithRole userWithRole)
        {

            try
            {
                var result = (from tb in _ctx.Users
                              where tb.IsActive &&
                              tb.UserID == userWithRole.UserID
                              select tb).FirstOrDefault();
                result.IsActive = false;
                result.ModifiedOn = DateTime.Now;
                result.ModifiedBy = userWithRole.ModifiedBy;
                await _ctx.SaveChangesAsync();

                //Changing the Status of role related to the user
                UserRoleMap UserRole = _ctx.UserRoleMaps.Where(x => x.UserID == userWithRole.UserID && x.IsActive == true).FirstOrDefault();
                UserRole.IsActive = false;
                UserRole.ModifiedOn = DateTime.Now;
                UserRole.ModifiedBy = userWithRole.ModifiedBy;
                await _ctx.SaveChangesAsync();

                //Delete the Profiles related to the user               
                await _ctx.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion


        #region Roles

        public async Task<bool> CreateRole(RoleWithApp roleWithApp)
        {
            try
            {
                Role role1 = (from tb in _ctx.Roles
                              where tb.IsActive && tb.RoleName == roleWithApp.RoleName
                              select tb).FirstOrDefault();
                if (role1 == null)
                {
                    var role = _mapper.Map<Role>(roleWithApp);
                    role.RoleID = Guid.NewGuid();
                    role.IsActive = true;
                    var result = _ctx.Roles.Add(role);
                    await _ctx.SaveChangesAsync();

                    foreach (int AppID in roleWithApp.AppIDList)
                    {
                        RoleAppMap roleApp = new RoleAppMap()
                        {
                            AppID = AppID,
                            RoleID = role.RoleID,
                            IsActive = true,
                            CreatedOn = DateTime.Now
                        };
                        _ctx.RoleAppMaps.Add(roleApp);
                    }
                    await _ctx.SaveChangesAsync();

                }
                else
                {
                    throw new Exception("Role with same name already exist");                   
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<RoleWithApp> GetAllRoles()
        {
            try
            {
                //List<RoleWithApp> RoleWithAppList = new List<RoleWithApp>();
                List<Role> RoleList = (from tb in _ctx.Roles
                                       where tb.IsActive
                                       select tb).ToList();
                var roleWithAppList = _mapper.Map<List<RoleWithApp>>(RoleList);
                //foreach (Role rol in RoleList)
                //{
                //    RoleWithAppList.Add(new RoleWithApp()
                //    {
                //        RoleID = rol.RoleID,
                //        RoleName = rol.RoleName,
                //        IsActive = rol.IsActive,
                //        CreatedOn = rol.CreatedOn,
                //        ModifiedOn = rol.ModifiedOn,
                //        AppIDList = _ctx.RoleAppMaps.Where(x => x.RoleID == rol.RoleID && x.IsActive == true).Select(r => r.AppID).ToArray()
                //    });
                //}
                return roleWithAppList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateRole(RoleWithApp roleWithApp)
        {
            try
            {

                Role role = (from tb in _ctx.Roles
                             where tb.IsActive && tb.RoleName == roleWithApp.RoleName && tb.RoleID != roleWithApp.RoleID
                             select tb).FirstOrDefault();
                if (role == null)
                {
                    Role role1 = (from tb in _ctx.Roles
                                  where tb.IsActive && tb.RoleID == roleWithApp.RoleID
                                  select tb).FirstOrDefault();
                    role1.RoleName = roleWithApp.RoleName;
                    role1.IsActive = true;
                    role1.ModifiedOn = DateTime.Now;
                    role1.ModifiedBy = roleWithApp.ModifiedBy;
                    await _ctx.SaveChangesAsync();

                    List<RoleAppMap> OldRoleAppList = _ctx.RoleAppMaps.Where(x => x.RoleID == roleWithApp.RoleID && x.IsActive == true).ToList();
                    List<RoleAppMap> NeedToRemoveRoleAppList = OldRoleAppList.Where(x => !roleWithApp.AppIDList.Any(y => y == x.AppID)).ToList();
                    List<int> NeedToAddAppList = roleWithApp.AppIDList.Where(x => !OldRoleAppList.Any(y => y.AppID == x)).ToList();

                    //Delete Old RoleApps which is not exist in new List
                    NeedToRemoveRoleAppList.ForEach(x =>
                    {
                        _ctx.RoleAppMaps.Remove(x);
                    });
                    await _ctx.SaveChangesAsync();

                    //Create New RoleApps
                    foreach (int AppID in NeedToAddAppList)
                    {
                        RoleAppMap roleApp = new RoleAppMap()
                        {
                            AppID = AppID,
                            RoleID = role1.RoleID,
                            IsActive = true,
                            CreatedOn = DateTime.Now,
                        };
                        _ctx.RoleAppMaps.Add(roleApp);
                    }
                    await _ctx.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception("Role with same name already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<bool> DeleteRole(RoleWithApp roleWithApp)
        {
            try
            {              
                Role role1 = (from tb in _ctx.Roles
                              where tb.IsActive && tb.RoleID == roleWithApp.RoleID
                              select tb).FirstOrDefault();
                role1.IsActive = false;
                role1.ModifiedOn = DateTime.Now;
                await _ctx.SaveChangesAsync();

                //Change the status of the RoleApps related to the role
                List<RoleAppMap> RoleAppList = _ctx.RoleAppMaps.Where(x => x.RoleID == roleWithApp.RoleID && x.IsActive == true).ToList();
                RoleAppList.ForEach(x =>
                {
                    x.IsActive = false;
                    x.ModifiedOn = DateTime.Now;
                    x.ModifiedBy = roleWithApp.ModifiedBy;
                });
                await _ctx.SaveChangesAsync();
                return true;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        #endregion

        #region Apps

        public async Task<App> CreateApp(App App)
        {
            try
            {
                App App1 = (from tb in _ctx.Apps
                            where tb.IsActive && tb.AppName == App.AppName
                            select tb).FirstOrDefault();
                if (App1 == null)
                {
                    App.CreatedOn = DateTime.Now;
                    App.IsActive = true;
                    var result = _ctx.Apps.Add(App);
                    await _ctx.SaveChangesAsync();
                    return App1;
                }
                else
                {
                   throw new Exception("App with same name already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public List<App> GetAllApps()
        {
            try
            {
                var result = (from tb in _ctx.Apps
                              where tb.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<App> UpdateApp(App App)
        {
            try
            {
                App App1 = (from tb in _ctx.Apps
                            where tb.IsActive && tb.AppName == App.AppName && tb.AppID != App.AppID
                            select tb).FirstOrDefault();
                if (App1 == null)
                {
                    App App2 = (from tb in _ctx.Apps
                                where tb.IsActive && tb.AppID == App.AppID
                                select tb).FirstOrDefault();
                    App2.AppName = App.AppName;
                    App2.IsActive = true;
                    App2.ModifiedOn = DateTime.Now;
                    App2.ModifiedBy = App.ModifiedBy;
                    await _ctx.SaveChangesAsync();
                    return App2;
                }
                else
                {
                    throw new Exception("App with same name already exist");                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }          
        }

        public async Task<App> DeleteApp(App App)
        {
            try
            {
                App App1 = (from tb in _ctx.Apps
                            where tb.IsActive && tb.AppID == App.AppID
                            select tb).FirstOrDefault();
                App1.IsActive = false;
                App1.ModifiedOn = DateTime.Now;
                App1.ModifiedBy = App.ModifiedBy;
                await _ctx.SaveChangesAsync();
                return App1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region User-Manual

        public async Task<bool> AddUserManual(IFormFileCollection files)
        {
            using (var transaction = _ctx.Database.BeginTransaction())
            {
                try
                {          
                    if (files.Count > 0)
                    {
                        IFormFile postedFile = files.FirstOrDefault();
                        UserManualDoc userManualDoc = new UserManualDoc();
                        userManualDoc.Id = 0;
                        userManualDoc.DocumentName = postedFile.FileName;
                        userManualDoc.CreatedOn = DateTime.Now;

                        UserManualDocStore userManualDocStore = new UserManualDocStore();

                        using (Stream st = postedFile.OpenReadStream())
                        {
                            using (BinaryReader br = new BinaryReader(st))
                            {
                                byte[] fileBytes = br.ReadBytes((Int32)st.Length);
                                if (fileBytes.Length > 0)
                                {
                                    userManualDocStore.Id = 0;
                                    userManualDocStore.FileName = postedFile.FileName;
                                    userManualDocStore.FileType = postedFile.ContentType;
                                    userManualDocStore.FileSize = postedFile.Length.ToString();
                                    userManualDocStore.FileContent = fileBytes;
                                    userManualDocStore.UserManualDocs = null;
                                }
                            }
                        }
                        var doc = this._ctx.UserManualDocStores.FirstOrDefault(x => x.FileType == userManualDocStore.FileType);
                        if (doc == null)
                        {
                            _ctx.UserManualDocs.Add(userManualDoc);
                            int insertUserManual = 0;
                            try
                            {
                                insertUserManual = _ctx.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            if (insertUserManual > 0)
                            {
                                userManualDocStore.UserManualDocId = userManualDoc.Id;
                                _ctx.UserManualDocStores.Add(userManualDocStore);
                                int insertDocStore = _ctx.SaveChanges();
                                if (insertDocStore > 0)
                                {
                                    transaction.Commit();
                                    transaction.Dispose();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    transaction.Dispose();
                                }
                            }
                        }
                        else
                        {
                            _ctx.UserManualDocs.Remove(_ctx.UserManualDocs.FirstOrDefault(u => u.Id == doc.UserManualDocId));
                            _ctx.UserManualDocs.Add(userManualDoc);
                            int insertUserManual = 0;
                            try
                            {
                                insertUserManual = _ctx.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            if (insertUserManual > 0)
                            {
                                userManualDocStore.UserManualDocId = userManualDoc.Id;
                                _ctx.UserManualDocStores.Add(userManualDocStore);
                                int insertDocStore = _ctx.SaveChanges();
                                if (insertDocStore > 0)
                                {
                                    transaction.Commit();
                                    transaction.Dispose();
                                }
                                else
                                {
                                    transaction.Rollback();
                                    transaction.Dispose();
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                transaction.Dispose();
                            }
                        }
                    }
                    return true;
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                    throw ex;
                }
            }
        }

        public async Task<List<UserManualDocStore>> GetUserManual()
        {
            try
            {
                var usermanual = _ctx.UserManualDocStores.ToList();
                if (usermanual.Count() > 0)
                {
                    return usermanual;
                }
                else
                {
                    return null;
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Reasons

        public async Task<Reason> CreateReason(Reason Reason)
        {
            try
            {
                Reason Reason1 = (from tb in _ctx.Reasons
                                  where tb.IsActive && tb.Description == Reason.Description
                                  select tb).FirstOrDefault();
                if (Reason1 == null)
                {
                    Reason.CreatedOn = DateTime.Now;
                    Reason.IsActive = true;
                    var result = _ctx.Reasons.Add(Reason);
                    await _ctx.SaveChangesAsync();
                    return Reason1;
                }
                else
                {
                    throw new Exception ("Reason with same name already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<List<Reason>> CreateBulkReason(List<ReasonData> reasonDatas)
        {
            List<Reason> addedReasons = new List<Reason>();
            try
            {
                foreach (var rsn in reasonDatas)
                {
                    Reason Reason1 = (from tb in _ctx.Reasons
                                      where tb.IsActive && tb.Description.ToLower() == rsn.ReasonDescription.ToLower()
                                      select tb).FirstOrDefault();
                    if (Reason1 == null)
                    {
                        Reason reason = new Reason();
                        reason.CreatedOn = DateTime.Now;
                        reason.IsActive = true;
                        reason.CreatedBy = "Admin";
                        reason.Description = rsn.ReasonDescription;
                        reason.ModifiedBy = "Admin";
                        reason.ModifiedOn = DateTime.Now;

                        var result = _ctx.Reasons.Add(reason);
                        await _ctx.SaveChangesAsync();
                        addedReasons.Add(reason);
                    }
                    else
                    {
                        throw new Exception("Reason with same name already exist");
                    }
                  
                }
                return addedReasons;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Reason> GetAllReasons()
        {
            try
            {
                var result = (from tb in _ctx.Reasons
                              where tb.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Reason> UpdateReason(Reason Reason)
        {
            try
            {
                Reason Reason1 = (from tb in _ctx.Reasons
                                  where tb.IsActive && tb.Description == Reason.Description && tb.ReasonID != Reason.ReasonID
                                  select tb).FirstOrDefault();
                if (Reason1 == null)
                {
                    Reason Reason2 = (from tb in _ctx.Reasons
                                      where tb.IsActive && tb.ReasonID == Reason.ReasonID
                                      select tb).FirstOrDefault();
                    Reason2.Description = Reason.Description;
                    Reason2.IsActive = true;
                    Reason2.ModifiedOn = DateTime.Now;
                    Reason2.ModifiedBy = Reason.ModifiedBy;
                    await _ctx.SaveChangesAsync();
                    return Reason2;
                }
                else
                {
                   throw new Exception("Reason with same name already exist");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<Reason> DeleteReason(Reason Reason)
        {
            try
            {
                Reason Reason1 = (from tb in _ctx.Reasons
                                  where tb.IsActive && tb.ReasonID == Reason.ReasonID
                                  select tb).FirstOrDefault();
                Reason1.IsActive = false;
                Reason1.ModifiedOn = DateTime.Now;
                Reason1.ModifiedBy = Reason.ModifiedBy;
                await _ctx.SaveChangesAsync();
                return Reason1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Organizations

        public async Task<Organization> CreateOrganization(Organization Organization)
        {
            try
            {   
                Organization Organization1 = (from tb in _ctx.Organizations
                                              where tb.IsActive && tb.OrganizationCode == Organization.OrganizationCode
                                              select tb).FirstOrDefault();
                if (Organization1 == null)
                {
                    Organization.CreatedOn = DateTime.Now;
                    Organization.IsActive = true;
                    var result = _ctx.Organizations.Add(Organization);
                    await _ctx.SaveChangesAsync();
                    return Organization;
                }
                else
                {
                   throw new Exception ("Organization with same code already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Organization> GetAllOrganizations()
        {
            try
            {
                var result = (from tb in _ctx.Organizations
                              where tb.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Organization> GetAllOrganizationsByUserID(Guid UserID)
        {
            try
            {
                var result = (from tb in _ctx.Organizations
                              join tb1 in _ctx.UserOrganizationMaps on tb.OrganizationCode equals tb1.OrganizationCode
                              where tb1.UserID == UserID && tb.IsActive && tb1.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Organization> UpdateOrganization(Organization Organization)
        {
            try
            {
                Organization Organization2 = (from tb in _ctx.Organizations
                                              where tb.IsActive && tb.OrganizationCode == Organization.OrganizationCode
                                              select tb).FirstOrDefault();
                Organization2.Description = Organization.Description;
                Organization2.IsActive = true;
                Organization2.ModifiedOn = DateTime.Now;
                Organization2.ModifiedBy = Organization.ModifiedBy;
                await _ctx.SaveChangesAsync();
                return Organization;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Organization> DeleteOrganization(Organization Organization)
        {
            try
            {
                Organization Organization1 = (from tb in _ctx.Organizations
                                              where tb.IsActive && tb.OrganizationCode == Organization.OrganizationCode
                                              select tb).FirstOrDefault();
                if (Organization1 != null)
                {
                    _ctx.Organizations.Remove(Organization1);
                }
                await _ctx.SaveChangesAsync();
                return Organization;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region CustomerGroup

        public async Task<CustomerGroup> CreateCustomerGroup(CustomerGroup userGroup)
        {
            try
            {
                CustomerGroup userGroup1 = (from tb in _ctx.CustomerGroups
                                            where tb.IsActive && tb.CustomerGroupCode == userGroup.CustomerGroupCode
                                            select tb).FirstOrDefault();
                if (userGroup1 == null)
                {
                    userGroup.CreatedOn = DateTime.Now;
                    userGroup.IsActive = true;
                    var result = _ctx.CustomerGroups.Add(userGroup);
                    await _ctx.SaveChangesAsync();
                    return userGroup1;
                }
                else
                {
                   throw new Exception("CustomerGroup with same code already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CustomerGroup GetSectorByCustomerGroup(string CustomerGroupCode)
        {
            try
            {
                var Result = _ctx.CustomerGroups.FirstOrDefault(t => t.CustomerGroupCode == CustomerGroupCode);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerGroup> GetAllCustomerGroups()
        {
            try
            {
                var result = (from tb in _ctx.CustomerGroups
                              where tb.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CustomerGroup> GetAllCustomerGroupsByUserID(Guid UserID)
        {
            try
            {
                var rolename = (from tb in _ctx.Users
                                join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                                join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID
                                where tb.UserID == UserID && tb.IsActive && tb1.IsActive && tb2.IsActive
                                select tb2.RoleName).FirstOrDefault();
                //  UserID = Guid.Parse(UserID.ToString().ToUpper());
                if (rolename.ToLower() == "administrator")
                {
                    var result = (from tb in _ctx.SLSGroupWithCustomerGroupMaps
                                  join tb1 in _ctx.UserSalesGroupMaps on tb.SGID equals tb1.SGID
                                  join tb2 in _ctx.CustomerGroups on tb.CGID equals tb2.CGID
                                  join tb3 in _ctx.Users on tb1.UserID equals tb3.UserID
                                  select tb2).Distinct().ToList();
                    return result;
                }
                else if (rolename.ToLower() != "customer")
                {
                    var result = (from tb in _ctx.SLSGroupWithCustomerGroupMaps
                                  join tb1 in _ctx.UserSalesGroupMaps on tb.SGID equals tb1.SGID
                                  join tb2 in _ctx.CustomerGroups on tb.CGID equals tb2.CGID
                                  join tb3 in _ctx.Users on tb1.UserID equals tb3.UserID

                                  where (tb3.UserID == UserID)
                                  select tb2).Distinct().ToList();
                    if (result.Count == 0)
                    {
                        result = _ctx.CustomerGroups.Where(k => k.IsActive).Distinct().ToList();
                    }
                    return result;
                }
                else
                {
                    return new List<CustomerGroup>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerGroup> UpdateCustomerGroup(CustomerGroup userGroup)
        {
            try
            {
                CustomerGroup userGroup2 = (from tb in _ctx.CustomerGroups
                                            where tb.IsActive && tb.CGID == userGroup.CGID
                                            select tb).FirstOrDefault();

                userGroup2.IsActive = true;
                userGroup2.CustomerGroupCode = userGroup.CustomerGroupCode;
                userGroup2.Sector = userGroup.Sector;
                userGroup2.ModifiedOn = DateTime.Now;
                userGroup2.ModifiedBy = userGroup.ModifiedBy;
                await _ctx.SaveChangesAsync();
                return userGroup2;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CreateBulkCustomerGroup(List<CustomerGroupData> cgDatas)
        {
            try
            {
                foreach (var cg in cgDatas)
                {
                    LogWriter.WriteToFile("Master/CreateBulkCustomergroup:- " + "Trying to insert " + cg.customergroupCode);
                    CustomerGroup cg1 = (from tb in _ctx.CustomerGroups
                                         where tb.IsActive && tb.CustomerGroupCode == cg.customergroupCode
                                         select tb).FirstOrDefault();
                    if (cg1 == null)
                    {
                        CustomerGroup customerGroup = new CustomerGroup();
                        customerGroup.CreatedOn = DateTime.Now;
                        customerGroup.IsActive = true;
                        customerGroup.CreatedBy = "Admin";

                        customerGroup.ModifiedBy = "Admin";
                        customerGroup.ModifiedOn = DateTime.Now;
                        customerGroup.CustomerGroupCode = cg.customergroupCode;
                        customerGroup.Sector = cg.sector;

                        var result = _ctx.CustomerGroups.Add(customerGroup);
                        await _ctx.SaveChangesAsync();
                        LogWriter.WriteToFile("Master/CreateBulkCustomergroup:- " + cg.customergroupCode + "inserted successfully");
                    }
                    else
                    {
                        cg1.Sector = cg.sector;
                        cg1.ModifiedBy = "Admin";
                        cg1.ModifiedOn = DateTime.Now;
                        await _ctx.SaveChangesAsync();
                        LogWriter.WriteToFile("Master/CreateBulkOrganization:- " + cg.customergroupCode + " already exists");
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CustomerGroup> DeleteCustomerGroup(CustomerGroup userGroup)
        {
            try
            {
                CustomerGroup userGroup1 = (from tb in _ctx.CustomerGroups
                                            where tb.IsActive && tb.CGID == userGroup.CGID
                                            select tb).FirstOrDefault();
                if (userGroup1 != null)
                {
                    _ctx.CustomerGroups.Remove(userGroup1);
                }
                await _ctx.SaveChangesAsync();
                return userGroup1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region ReversePOD

        public List<UserWithRole> GetAllDCUsers()
        {
            try
            {
                var noRecords = 20;
                var TakeValue = noRecords;
                var result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID
                              where tb.IsActive && tb1.IsActive == true && tb2.RoleName == "Amararaja User"
                              orderby tb.CreatedOn
                              select new
                              {
                                  tb.UserID,
                                  tb.UserCode,
                                  tb.UserName,
                                  tb.Email,
                                  tb.ContactNumber,
                                  tb.Password,
                                  tb.IsActive,
                                  tb.CreatedOn,
                                  tb.ModifiedOn,
                                  tb1.RoleID,
                                  tb.CustomerGroupCode
                              }).ToList();

                List<UserWithRole> UserWithRoleList = new List<UserWithRole>();

                result.ForEach(record =>
                {
                    UserWithRoleList.Add(new UserWithRole()
                    {
                        UserID = record.UserID,
                        UserCode = record.UserCode,
                        UserName = record.UserName,
                        OrganizationList = (from tb in _ctx.UserOrganizationMaps
                                            join tb1 in _ctx.Organizations on tb.OrganizationCode equals tb1.OrganizationCode
                                            where tb.UserID == record.UserID
                                            select tb.OrganizationCode).ToList(),
                        PlantList = (from tb in _ctx.UserPlantMaps
                                     join tb1 in _ctx.Plants on tb.PlantCode equals tb1.PlantCode
                                     where tb.UserID == record.UserID
                                     select tb.PlantCode).ToList(),
                        Email = record.Email,
                        ContactNumber = record.ContactNumber,
                        Password =_passwordEncryptor.Decrypt(record.Password, true),
                        IsActive = record.IsActive,
                        CreatedOn = record.CreatedOn,
                        ModifiedOn = record.ModifiedOn,
                        RoleID = record.RoleID,
                        CustomerGroup = record.CustomerGroupCode,
                        SLSgroups = _ctx.UserSalesGroupMaps.Where(k => k.UserID == record.UserID).Select(p => p.SGID).ToList()

                    });

                });
                return UserWithRoleList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region SalesGroup

        public async Task<bool> CreateSalesGroup(SLSCustGroupData slsGroup)
        {
            try
            {
                SLSGroupWithCustomerGroupMap chk = _ctx.SLSGroupWithCustomerGroupMaps.FirstOrDefault(k => k.SLSGroupCode == slsGroup.SLSGroupCode);
                if (chk == null)
                {
                    var connection = _ctx.Database.GetDbConnection();

                    connection.Open();
                    var cmd = connection.CreateCommand();

                    cmd.CommandText = String.Format("SELECT NEXT VALUE FOR[{0}].[dbo].[IncrSLSId];", connection.Database);
                    var obj = await cmd.ExecuteScalarAsync();
                    int anInt = int.Parse(obj.ToString());
                    connection.Close();
                    slsGroup.CustomerGroupCode.ForEach(cg =>
                    {
                        SLSGroupWithCustomerGroupMap customerGroupMap = new SLSGroupWithCustomerGroupMap();
                        customerGroupMap.CreatedOn = DateTime.Now;
                        customerGroupMap.CGID = cg;
                        customerGroupMap.IsActive = true;
                        customerGroupMap.CreatedBy = slsGroup.CreatedBy;
                        customerGroupMap.Description = slsGroup.Description;
                        customerGroupMap.SLSGroupCode = slsGroup.SLSGroupCode;
                        customerGroupMap.SGID = anInt;
                        _ctx.SLSGroupWithCustomerGroupMaps.Add(customerGroupMap);

                    });
                    await _ctx.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception("Same Sales Goup already exists");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SLSCustGroupData> GetAllSalesGroups()
        {
            try
            {
                List<SLSCustGroupData> sLSCust = new List<SLSCustGroupData>();
                List<int> slscode = (from tb in _ctx.SLSGroupWithCustomerGroupMaps
                                     where tb.IsActive
                                     select tb.SGID).Distinct().ToList();

                slscode.ForEach(p =>
                {
                    SLSGroupWithCustomerGroupMap temp = _ctx.SLSGroupWithCustomerGroupMaps.Where(l => l.SGID == p).FirstOrDefault();
                    SLSCustGroupData custGroupData = new SLSCustGroupData();
                    custGroupData.SLSGroupCode = temp.SLSGroupCode;
                    custGroupData.SGID = temp.SGID;
                    custGroupData.CreatedOn = temp.CreatedOn;
                    custGroupData.CreatedBy = temp.CreatedBy;
                    custGroupData.Description = temp.Description;
                    custGroupData.CustomerGroupCode = _ctx.SLSGroupWithCustomerGroupMaps.Where(l => l.SGID == temp.SGID).Select(u => u.CGID).ToList();
                    custGroupData.ModifiedBy = temp.ModifiedBy;
                    custGroupData.ModifiedOn = temp.ModifiedOn;
                    sLSCust.Add(custGroupData);
                });

                return sLSCust;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateSalesGroup(SLSCustGroupData slsGroup)
        {
            try
            {
                _ctx.SLSGroupWithCustomerGroupMaps.RemoveRange(_ctx.SLSGroupWithCustomerGroupMaps.Where(k => k.SGID == slsGroup.SGID));

                await _ctx.SaveChangesAsync();

                slsGroup.CustomerGroupCode.ForEach(cg =>
                {
                    SLSGroupWithCustomerGroupMap customerGroupMap = new SLSGroupWithCustomerGroupMap();
                    customerGroupMap.CreatedOn = DateTime.Now;
                    customerGroupMap.CGID = cg;
                    customerGroupMap.SGID = slsGroup.SGID;
                    customerGroupMap.IsActive = true;
                    customerGroupMap.CreatedBy = slsGroup.CreatedBy;
                    customerGroupMap.Description = slsGroup.Description;
                    customerGroupMap.SLSGroupCode = slsGroup.SLSGroupCode;
                    _ctx.SLSGroupWithCustomerGroupMaps.Add(customerGroupMap);
                });

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SLSGroupWithCustomerGroupMap>> CreateBulkSalesGroup(List<SLSGroupWithCustomerGroupMap> sgDatas)
        {
            var addedGroups = new List<SLSGroupWithCustomerGroupMap>();
            try
            {
                foreach (var cg in sgDatas)
                {
                    LogWriter.WriteToFile("Master/CreateBulkCustomergroup:- " + "Trying to insert " + cg.SLSGroupCode);
                    SLSGroupWithCustomerGroupMap cg1 = (from tb in _ctx.SLSGroupWithCustomerGroupMaps
                                                        where tb.IsActive && tb.SLSGroupCode == cg.SLSGroupCode
                                                        select tb).FirstOrDefault();
                    if (cg1 == null)
                    {
                        SLSGroupWithCustomerGroupMap customerGroup = new SLSGroupWithCustomerGroupMap();
                        customerGroup.CreatedOn = DateTime.Now;
                        customerGroup.IsActive = true;
                        customerGroup.CreatedBy = "Admin";

                        customerGroup.ModifiedBy = "Admin";
                        customerGroup.ModifiedOn = DateTime.Now;
                        customerGroup.CustomerGroupCode = cg.SLSGroupCode;
                        var result = _ctx.SLSGroupWithCustomerGroupMaps.Add(customerGroup);
                        await _ctx.SaveChangesAsync();
                        addedGroups.Add(customerGroup);
                        LogWriter.WriteToFile("Master/CreateBulkCustomergroup:- " + cg.SLSGroupCode + "inserted successfully");
                       
                    }
                    else
                    {
                        LogWriter.WriteToFile("Master/CreateBulkOrganization:- " + cg.SLSGroupCode + " already exists");
                    }
                }
                return addedGroups;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<SLSGroupWithCustomerGroupMap>> DeleteSalesGroup(SLSCustGroupData sLsGroup)
        {
            try
            {
                List<SLSGroupWithCustomerGroupMap> userGroup1 = (from tb in _ctx.SLSGroupWithCustomerGroupMaps
                                                                 where tb.IsActive && tb.SGID == sLsGroup.SGID
                                                                 select tb).ToList();
                if (userGroup1 != null)
                {
                    _ctx.SLSGroupWithCustomerGroupMaps.RemoveRange(userGroup1);
                }
                await _ctx.SaveChangesAsync();
                return userGroup1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region TEST
        public async Task<bool> PushUserCreationLog()
        {
            try
            {
                var result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              where tb.IsActive && tb1.IsActive == true
                              orderby tb.CreatedOn
                              select new
                              {
                                  tb.UserID,
                                  tb.UserCode,
                                  tb.UserName,
                                  tb.Email,
                                  tb.ContactNumber,
                                  tb.Password,
                                  tb.IsActive,
                                  tb.CreatedOn,
                                  tb.ModifiedOn,
                                  tb1.RoleID,
                                  tb.CustomerGroupCode
                              }).ToList();

                List<UserWithRole> UserWithRoleList = new List<UserWithRole>();

                result.ForEach(record =>
                {
                    UserWithRoleList.Add(new UserWithRole()
                    {
                        UserID = record.UserID,
                        UserCode = record.UserCode,
                        UserName = record.UserName,
                        OrganizationList = (from tb in _ctx.UserOrganizationMaps
                                            join tb1 in _ctx.Organizations on tb.OrganizationCode equals tb1.OrganizationCode
                                            where tb.UserID == record.UserID
                                            select tb.OrganizationCode).ToList(),
                        PlantList = (from tb in _ctx.UserPlantMaps
                                     join tb1 in _ctx.Plants on tb.PlantCode equals tb1.PlantCode
                                     where tb.UserID == record.UserID
                                     select tb.PlantCode).ToList(),
                        Email = record.Email,
                        ContactNumber = record.ContactNumber,
                        Password =_passwordEncryptor.Decrypt(record.Password, true),
                        IsActive = record.IsActive,
                        CreatedOn = record.CreatedOn,
                        ModifiedOn = record.ModifiedOn,
                        RoleID = record.RoleID,
                        CustomerGroup = record.CustomerGroupCode

                    });

                });
                UserWithRoleList.ForEach(u =>
                {
                    UserCreationErrorLog errorLog = new UserCreationErrorLog();
                    errorLog.ContactNo = u.ContactNumber;
                    errorLog.Date = result.Find(x => x.UserID == u.UserID).CreatedOn;
                    errorLog.Email = u.Email;
                    errorLog.LogReson = "Created";
                    errorLog.RoleName = _ctx.Roles.ToList().Find(h => h.RoleID == u.RoleID).RoleName;
                    errorLog.UserCode = u.UserCode;
                    errorLog.UserName = u.UserName;
                    _ctx.UserCreationErrorLogs.Add(errorLog);
                });
                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #endregion

        #region Plants
        public async Task<bool> CreatePlant(PlantWithOrganization plantWithOrganization)
        {
            try
            {
                Plant Plant1 = (from tb in _ctx.Plants
                                where tb.IsActive && tb.PlantCode == plantWithOrganization.PlantCode
                                select tb).FirstOrDefault();
                if (Plant1 == null)
                {
                    Plant Plant = new Plant();
                    Plant.PlantCode = plantWithOrganization.PlantCode;
                    Plant.Description = plantWithOrganization.Description;
                    Plant.CreatedBy = plantWithOrganization.CreatedBy;
                    Plant.CreatedOn = DateTime.Now;
                    Plant.IsActive = true;
                    var result = _ctx.Plants.Add(Plant);
                    await _ctx.SaveChangesAsync();
                    if (plantWithOrganization.OrganizationCode != null)
                    {
                        PlantOrganizationMap plantOrganizationMap = new PlantOrganizationMap()
                        {
                            PlantCode = Plant.PlantCode,
                            OrganizationCode = plantWithOrganization.OrganizationCode,
                            IsActive = true,
                            CreatedOn = DateTime.Now
                        };
                        var r1 = _ctx.PlantOrganizationMaps.Add(plantOrganizationMap);
                    }
                    await _ctx.SaveChangesAsync();
                    return true;
                }
                else
                {
                   throw new Exception("Plant with same code already exist");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }

        public List<PlantWithOrganization> GetAllPlants()
        {
            try
            {
                var result = (from tb in _ctx.Plants
                              where tb.IsActive
                              select tb).ToList();

                List<PlantWithOrganization> PlantWithOrganizationList = new List<PlantWithOrganization>();

                result.ForEach(record =>
                {
                    PlantWithOrganizationList.Add(new PlantWithOrganization()
                    {
                        PlantCode = record.PlantCode,
                        Description = record.Description,
                        OrganizationCode = (from tb in _ctx.PlantOrganizationMaps
                                            join tb1 in _ctx.Organizations on tb.OrganizationCode equals tb1.OrganizationCode
                                            where tb.PlantCode == record.PlantCode
                                            select tb.OrganizationCode).FirstOrDefault(),
                        IsActive = record.IsActive,
                        CreatedBy = record.CreatedBy,
                        CreatedOn = record.CreatedOn,
                        ModifiedOn = record.ModifiedOn,
                        ModifiedBy = record.ModifiedBy,
                    });

                });
                return PlantWithOrganizationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PlantWithOrganization> GetAllPlantsByUserID(Guid UserID)
        {
            try
            {
                var result = (from tb in _ctx.Plants
                              join tb1 in _ctx.UserPlantMaps on tb.PlantCode equals tb1.PlantCode
                              where tb1.UserID == UserID && tb.IsActive && tb1.IsActive
                              select tb).ToList();

                List<PlantWithOrganization> PlantWithOrganizationList = new List<PlantWithOrganization>();

                result.ForEach(record =>
                {
                    PlantWithOrganizationList.Add(new PlantWithOrganization()
                    {
                        PlantCode = record.PlantCode,
                        Description = record.Description,
                        OrganizationCode = (from tb in _ctx.PlantOrganizationMaps
                                            join tb1 in _ctx.Organizations on tb.OrganizationCode equals tb1.OrganizationCode
                                            where tb.PlantCode == record.PlantCode
                                            select tb.OrganizationCode).FirstOrDefault(),
                        IsActive = record.IsActive,
                        CreatedBy = record.CreatedBy,
                        CreatedOn = record.CreatedOn,
                        ModifiedOn = record.ModifiedOn,
                        ModifiedBy = record.ModifiedBy,
                    });

                });
                return PlantWithOrganizationList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PlantGroup> GetAllPlantGroups()
        {
            try
            {
                return _ctx.PlantGroups.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdatePlant(PlantWithOrganization plantWithOrganization)
        {
            try
            {
                Plant Plant2 = (from tb in _ctx.Plants
                                where tb.IsActive && tb.PlantCode == plantWithOrganization.PlantCode
                                select tb).FirstOrDefault();
                Plant2.Description = plantWithOrganization.Description;
                Plant2.IsActive = true;
                Plant2.ModifiedOn = DateTime.Now;
                Plant2.ModifiedBy = plantWithOrganization.ModifiedBy;
                await _ctx.SaveChangesAsync();

                _ctx.PlantOrganizationMaps.Where(x => x.PlantCode == plantWithOrganization.PlantCode).ToList().ForEach(y => _ctx.PlantOrganizationMaps.Remove(y));
                await _ctx.SaveChangesAsync();

                if (plantWithOrganization.OrganizationCode != null)
                {
                    PlantOrganizationMap plantOrganizationMap = new PlantOrganizationMap()
                    {
                        PlantCode = Plant2.PlantCode,
                        OrganizationCode = plantWithOrganization.OrganizationCode,
                        IsActive = true,
                        CreatedOn = DateTime.Now
                    };
                    var r1 = _ctx.PlantOrganizationMaps.Add(plantOrganizationMap);                    
                }
                await _ctx.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async Task<Plant> DeletePlant(Plant Plant)
        {
            try
            {
                Plant Plant1 = (from tb in _ctx.Plants
                                where tb.IsActive && tb.PlantCode == Plant.PlantCode
                                select tb).FirstOrDefault();
                if (Plant1 != null)
                {
                    _ctx.Plants.Remove(Plant1);
                    // var plorg = (_ctx.PlantOrganizationMaps.Where(k => k.PlantCode == Plant1.PlantCode)).ToList();
                    _ctx.PlantOrganizationMaps.RemoveRange((_ctx.PlantOrganizationMaps.Where(k => k.PlantCode == Plant1.PlantCode)));
                }
                await _ctx.SaveChangesAsync();
                return Plant1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        public List<PlantOrganizationMap> GetAllPlantOrganizationMaps()
        {
            try
            {
                var result = (from tb in _ctx.PlantOrganizationMaps
                              where tb.IsActive
                              select tb).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region grouping & download

        public List<RolewithGroup> GetRoleandGroups()
        {
            try
            {
                var grps = (from tb in _ctx.Users
                            join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                            join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID
                            join tb3 in _ctx.CustomerGroups on tb.CustomerGroupCode equals tb3.CustomerGroupCode
                            select new
                            {
                                RoleName = tb2.RoleName,
                                CustomerGroup = tb.CustomerGroupCode,

                                RoleID = tb1.RoleID,
                                UserID = tb.UserID
                            });
                List<RolewithGroup> rolewithGroup = new List<RolewithGroup>();
                foreach (var g in grps)
                {
                    var rwg = new RolewithGroup();
                    rwg.CustomerGroup = g.CustomerGroup;

                    rwg.RoleID = g.RoleID;
                    rwg.RoleName = g.RoleName;
                    rwg.UserID = g.UserID;

                    rolewithGroup.Add(rwg);

                }
                return rolewithGroup;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region LogInAndChangePassword

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
                //foreach (IPAddress addr in localIPs)
                //{
                //    loginData.IP += addr + ",";
                //}
                _ctx.UserLoginHistories.Add(loginData);
                await _ctx.SaveChangesAsync();
                return loginData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserLoginHistory> GetAllUsersLoginHistory()
        {
            try
            {
                var UserLoginHistoryList = (from tb1 in _ctx.UserLoginHistories
                                            orderby tb1.LoginTime descending
                                            select tb1).ToList();

                return UserLoginHistoryList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserLoginHistory> GetCurrentUserLoginHistory(Guid UserID)
        {
            try
            {
                var UserLoginHistoryList = (from tb1 in _ctx.UserLoginHistories
                                            where tb1.UserID == UserID
                                            orderby tb1.LoginTime descending
                                            select tb1).ToList();
                return UserLoginHistoryList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UserLoginHistory> SignOut(Guid UserID)
        {
            try
            {
                var result = _ctx.UserLoginHistories.Where(data => data.UserID == UserID).OrderByDescending(d => d.LoginTime).FirstOrDefault();
                result.LogoutTime = DateTime.Now;
                await _ctx.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }         
        }

        #endregion

        #region ChangePassword
        public async Task<User> ChangePassword(ChangePassword changePassword)
        {
            try
            {
                User user = (from tb in _ctx.Users
                             where tb.UserID == changePassword.UserID && tb.IsActive
                             select tb).FirstOrDefault();
                if (user != null)
                {
                    string DecryptedPassword =_passwordEncryptor.Decrypt(user.Password, true);
                    if (DecryptedPassword == changePassword.CurrentPassword)
                    {
                        string DefaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
                        if (changePassword.NewPassword == DefaultPassword)
                        {
                            throw new Exception("New password should be different from default password.");
                        }
                        else
                        {
                            user.FourthLastPassword = user.ThirdLastPassword;
                            user.ThirdLastPassword = user.SecondLastPassword;
                            user.SecondLastPassword = user.LastPassword;
                            user.LastPassword = user.Password;

                            user.Password =_passwordEncryptor.Encrypt(changePassword.NewPassword, true);
                            user.IsActive = true;
                            user.IsLocked = false;
                            user.ModifiedOn = DateTime.Now;
                            user.LastPasswordChangeDate = DateTime.Now;

                            await _ctx.SaveChangesAsync();
                            return user;
                        }
                    }
                    else
                    {
                        throw new Exception("Current password is incorrect.");
                    }
                }
                else
                {
                    throw new Exception("The user name or password is incorrect.");
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }           
        }

        public async Task<bool> SendResetLinkToMail(EmailModel emailModel)
        {
            try
            {
                DateTime ExpireDateTime = DateTime.Now.AddMinutes(_tokenTimespan);
                User user = (from tb in _ctx.Users
                             where tb.UserCode == emailModel.UserName && tb.IsActive
                             select tb).FirstOrDefault();

                if (user != null)
                {
                    if (user.IsLocked == true)
                    {
                        throw new Exception("User account is locked..");
                    }
                    string code =_passwordEncryptor.Encrypt(user.UserID.ToString() + '|' + user.UserName + '|' + ExpireDateTime, true);

                    bool sendresult = await _emailHelper.SendMail(HttpUtility.UrlEncode(code), user.UserName, user.Email, null, "", user.UserID.ToString(), emailModel.siteURL);
                    if (sendresult)
                    {
                        try
                        {
                            TokenHistory history1 = (from tb in _ctx.TokenHistories
                                                     where tb.UserID == user.UserID && !tb.IsUsed
                                                     select tb).FirstOrDefault();
                            if (history1 == null)
                            {
                                TokenHistory history = new TokenHistory()
                                {
                                    UserID = user.UserID,
                                    Token = code,
                                    EmailAddress = user.Email,
                                    CreatedOn = DateTime.Now,
                                    ExpireOn = ExpireDateTime,
                                    IsUsed = false,
                                    Comment = "Token sent successfully"
                                };
                                var result = _ctx.TokenHistories.Add(history);
                            }
                            else
                            {
                                LogWriter.WriteToFile("Master/SendLinkToMail:- Token already present, updating new token to the user whose mail id is " + user.Email);
                                history1.Token = code;
                                history1.CreatedOn = DateTime.Now;
                                history1.ExpireOn = ExpireDateTime;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                      //  return Content(HttpStatusCode.OK, string.Format("Reset password link sent successfully to {0}", user.Email));
                    }
                    else
                    {
                        throw new Exception("Sorry! There is some problem on sending mail");
                    }
                }
                else
                {
                    throw new Exception("Your email address is not registered!");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async Task<User> SendOTPToMail(string UserCode)
        {
            try
            {
                DateTime ExpireDateTime = DateTime.Now.AddMinutes(_tokenTimespan);
                User user = (from tb in _ctx.Users
                             where tb.UserCode == UserCode && tb.IsActive
                             select tb).FirstOrDefault();

                if (user != null)
                {
                    string otp = GenerateOTP().ToString();

                    bool sendresult = await _emailHelper.SendOTPMail(otp, user.UserCode, user.Email);
                    if (sendresult)
                    {
                        try
                        {
                            TokenHistory history1 = (from tb in _ctx.TokenHistories
                                                     where tb.UserID == user.UserID && !tb.IsUsed
                                                     select tb).FirstOrDefault();
                            if (history1 == null)
                            {
                                TokenHistory history = new TokenHistory()
                                {
                                    UserID = user.UserID,
                                    OTP = otp,
                                    EmailAddress = user.Email,
                                    CreatedOn = DateTime.Now,
                                    ExpireOn = ExpireDateTime,
                                    IsUsed = false,
                                    Comment = "OTP sent successfully"
                                };
                                var result = _ctx.TokenHistories.Add(history);
                            }
                            else
                            {
                                LogWriter.WriteToFile("Master/SendOTPToMail:- OTP already present, updating new otp to the user whose mail id is " + user.Email);
                                history1.OTP = otp;
                                history1.CreatedOn = DateTime.Now;
                                history1.ExpireOn = ExpireDateTime;
                            }
                            await _ctx.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        //return Content(HttpStatusCode.OK, string.Format("OTP sent successfully to {0}", user.Email));
                    }
                    else
                    {
                        throw new Exception("Sorry! There is some problem on sending mail");
                    }
                }
                else
                {
                    throw new Exception("Your email address is not registered!");
                }
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ForgotPassword(ForgotPassword forgotPassword)
        {
            string[] decryptedArray = new string[3];
            string result = string.Empty;
            try
            {               
                try
                {
                    result =_passwordEncryptor.Decrypt(forgotPassword.Token, true);
                }
                catch
                {
                   throw new Exception("Invalid token!");
                   
                }
                if (result.Contains('|') && result.Split('|').Length == 3)
                {
                    decryptedArray = result.Split('|');
                }
                else
                {
                    throw new Exception("Invalid token!");
                }

                if (decryptedArray.Length == 3)
                {
                    DateTime date = DateTime.Parse(decryptedArray[2].Replace('+', ' '));
                    if (DateTime.Now > date)// Convert.ToDateTime(decryptedarray[2]))
                    {
                        throw new Exception("Reset password link expired!");
                    }
                    var DecryptedUserID = decryptedArray[0];

                    User user = (from tb in _ctx.Users
                                 where tb.UserID.ToString() == DecryptedUserID && tb.IsActive
                                 select tb).FirstOrDefault();

                    if (user.UserName == decryptedArray[1] && forgotPassword.UserID == user.UserID)
                    {
                        try
                        {
                            TokenHistory history = _ctx.TokenHistories.Where(x => x.UserID == user.UserID && !x.IsUsed && x.Token == forgotPassword.Token).Select(r => r).FirstOrDefault();
                            if (history != null)
                            {
                                // Updating Password
                                user.FourthLastPassword = user.ThirdLastPassword;
                                user.ThirdLastPassword = user.SecondLastPassword;
                                user.SecondLastPassword = user.LastPassword;
                                user.LastPassword = user.Password;

                                user.Password =_passwordEncryptor.Encrypt(forgotPassword.NewPassword, true);
                                user.IsActive = true;
                                user.ModifiedOn = DateTime.Now;
                                await _ctx.SaveChangesAsync();

                                // Updating TokenHistory
                                history.UsedOn = DateTime.Now;
                                history.IsUsed = true;
                                history.Comment = "Token Used successfully";
                                await _ctx.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception("Token might have already used or wrong token");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    else
                    {
                        throw new Exception("Invalid token!");
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async Task<bool> ChangePasswordUsingOTP(ForgotPasswordOTP forgotPassword)
        {
            string result = string.Empty;
            try
            {
                User user = (from tb in _ctx.Users
                             where tb.UserCode == forgotPassword.UserCode && tb.IsActive
                             select tb).FirstOrDefault();

                if (user != null)
                {
                    try
                    {
                        TokenHistory history = _ctx.TokenHistories.Where(x => x.UserID == user.UserID && x.OTP == forgotPassword.OTP).Select(r => r).FirstOrDefault();
                        if (history != null)
                        {
                            if (DateTime.Now > history.ExpireOn)
                            {
                                throw new Exception("The given OTP has been expired,Try creating new otp");
                            }
                            else
                            {
                                if (!history.IsUsed)
                                {
                                    // Updating Password
                                    user.Password =_passwordEncryptor.Encrypt(forgotPassword.NewPassword, true);
                                    user.IsActive = true;
                                    user.ModifiedOn = DateTime.Now;
                                    await _ctx.SaveChangesAsync();

                                    // Updating TokenHistory
                                    history.UsedOn = DateTime.Now;
                                    history.IsUsed = true;
                                    history.Comment = "OTP Used successfully";
                                    await _ctx.SaveChangesAsync();
                                }
                                else
                                {
                                    throw new Exception("The given OTP has already been used,Try creating new otp");
                                }
                            }

                        }
                        else
                        {
                            throw new Exception("The given OTP is invalid");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    throw new Exception("Invalid OTP");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        //public async Task<IActionResult> PasswordResetSendSMSOTP(resetPasswordOTPBody oTPBody)
        //{
        //    OTPResponseBody oTPResponse = new OTPResponseBody();
        //    try
        //    {
        //        User user = _ctx.Users.FirstOrDefault(x => (x.UserCode == oTPBody.UserName || x.UserCode == oTPBody.UserName));

        //        if (user != null)
        //        {
        //            if (user.IsLocked == true)
        //            {
        //                throw new Exception("User account is locked..");
        //            }
        //            else
        //            {
        //                LogWriter.WriteToFile("---------------------------------------------------------------------------------------Send OTP Process Starts------------------------------------------------------------------------------------------------------");
        //                // t Sendrequest = new SendOTPRequest();
        //                // SendOTPRequest Sendrequest = JsonConvert.DeserializeObject<SendOTPRequest>(SendOTPRequest);

        //                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SMSOTPAPI);

        //                request.Method = "POST";
        //                request.KeepAlive = true;
        //                request.AllowAutoRedirect = true;
        //                request.Accept = "*/*";
        //                Random generator = new Random();
        //                String r = generator.Next(0, 1000000).ToString("D6");
        //                string smsotp = "";
        //                smsotp = SMSOTPmsg.Replace("U$erN@me", user.UserName.Substring(0, 3));
        //                smsotp = smsotp.Replace("(o%^!)", r);
        //                SendOtpMtalkZPayload SendOTPPayload = new SendOtpMtalkZPayload
        //                {
        //                    apikey = SMSOTPapiKey,
        //                    senderid = SMSSenderID,
        //                    number = "91" + user.ContactNumber,
        //                    message = smsotp,
        //                    format = "json"
        //                };
        //                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        //                {
        //                    var pld = JsonConvert.SerializeObject(SendOTPPayload);
        //                    writer.Write(pld);
        //                    writer.Flush();
        //                    writer.Close();
        //                }
        //                string resJson1 = new StreamReader(request.GetResponse().GetResponseStream()).ReadToEnd();
        //                LogWriter.WriteToFile("Response " + resJson1);
        //                // resJson = JsonConvert.DeserializeObject<string>(resJson);
        //                string resJson = resJson1.Replace("\"", "");
        //                resJson = resJson.Replace("\n", "");

        //                dynamic SendOTPResponse = JsonConvert.DeserializeObject<dynamic>(resJson1);
        //                LogWriter.WriteToFile("Response Status : " + SendOTPResponse.status.ToString());
        //                if (SendOTPResponse.status.ToString() != "OK")
        //                {
        //                    LogWriter.WriteToFile("Received Error Response.................");
        //                    LogWriter.WriteToFile("---------------------------------------------------------------------------------------Send OTP Process ENDS------------------------------------------------------------------------------------------------------");

        //                    throw new Exception("Error Sending OTP:" + SendOTPResponse.message.ToString());
        //                    //throw new Exception("Error Sending OTP:" + SendOTPResponse.status);

        //                }

        //                SMSOTPChangePasswordHistory sMSOTPChangePassword = new SMSOTPChangePasswordHistory();
        //                sMSOTPChangePassword.IsPasswordChanged = false;
        //                sMSOTPChangePassword.OTP = r;
        //                sMSOTPChangePassword.OTPCreatedOn = DateTime.Now;
        //                sMSOTPChangePassword.OTPExpiredOn = DateTime.Now.AddMinutes(5);
        //                sMSOTPChangePassword.OTPID = SendOTPResponse.msgid.ToString();
        //                sMSOTPChangePassword.OTPUsedOn = null;
        //                sMSOTPChangePassword.UserName = user.UserName;
        //                sMSOTPChangePassword.MobileNumber = user.ContactNumber;
        //                sMSOTPChangePassword.IsOTPUSed = false;
        //                _ctx.SMSOTPChnagePasswordHistories.Add(sMSOTPChangePassword);

        //                if (await _ctx.SaveChangesAsync() > 0)
        //                {
        //                    oTPResponse.OTPtranID = SendOTPResponse.msgid.ToString();
        //                    oTPResponse.UserGuid = user.UserID;
        //                    return oTPResponse;
        //                }
        //                oTPResponse.OTPtranID = "-1";

        //                return oTPResponse;
        //            }

        //        }
        //        else
        //        {
        //            throw new Exception("No User Exits..");
        //            //throw new Exception("No User Exits..");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oTPResponse.OTPtranID = ex.Message;
        //        throw ex;
        //    }
        //}

        public async Task<IActionResult> ResetPasswordWithSMSOTP(AffrimativeOTPBody oTPBody)
        {
            try
            {
                SMSOTPChangePasswordHistory sMSOTP = _ctx.SMSOTPChnagePasswordHistories.FirstOrDefault(x => x.OTPID == oTPBody.OTPTransID && x.OTP == oTPBody.recievedOTP);

                if (sMSOTP != null)
                {
                    if (sMSOTP.OTPExpiredOn > DateTime.Now)
                    {
                        string encryptedPassword =_passwordEncryptor.Encrypt(oTPBody.newPassword, true);
                        User usr = _ctx.Users.FirstOrDefault(k => k.UserID == oTPBody.UserGuid);
                        usr.FourthLastPassword = usr.ThirdLastPassword;
                        usr.ThirdLastPassword = usr.SecondLastPassword;
                        usr.SecondLastPassword = usr.LastPassword;
                        usr.LastPassword = usr.Password;
                        usr.Password = encryptedPassword;

                        sMSOTP.IsOTPUSed = true;
                        sMSOTP.IsPasswordChanged = true;
                        sMSOTP.OTPUsedOn = DateTime.Now;
                        if (await _ctx.SaveChangesAsync() > 0)
                        {
                            throw new Exception("Success");
                        }
                        else
                        {
                            throw new Exception("Failed Updating Password..");
                        }
                    }
                    else
                    {
                        throw new Exception("OTP expired..");
                    }
                }
                else
                {
                    throw new Exception("OTP not matching..");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        public int GenerateOTP()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        #region Unlock User

        public async Task<bool> UnlockUser(Guid UserId)
        {
            try
            {
                var user = _ctx.Users.FirstOrDefault(x => x.UserID == UserId);
                if (user != null)
                {
                    user.IsLocked = false;
                    user.WrongAttempt = 0;
                    await _ctx.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<UserWithRole> GetAllLockedUsers()
        {
            try
            {
                var result = (from tb in _ctx.Users
                              join tb1 in _ctx.UserRoleMaps on tb.UserID equals tb1.UserID
                              join tb2 in _ctx.Roles on tb1.RoleID equals tb2.RoleID
                              where tb.IsLocked == true && tb.IsActive == true
                              select new UserWithRole
                              {
                                  UserID = tb.UserID,
                                  RoleID = tb1.RoleID,
                                  UserName = tb.UserName,
                                  UserCode = tb.UserCode,
                                  Email = tb.Email,
                                  IsActive = tb.IsActive
                              }).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
