using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Dtos.Auth;
using AREML.EPOD.Core.Dtos.Response;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Core.Entities.Logs;
using AREML.EPOD.Core.Entities.Master;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AREML.EPOD.Data.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly AuthContext _dbContext;
        private readonly JwtSetting _jwtSetting;
        private readonly PasswordEncryptor _passwordEncryptor;
        private readonly IMasterRepository _masterRepository;

        public AuthenticationRepository(AuthContext context, IConfiguration config, PasswordEncryptor passwordEncryptor, IMasterRepository masterRepository)
        {
            this._dbContext = context;
            _jwtSetting = config.GetSection("JWTSecurity").Get<JwtSetting>();
            _passwordEncryptor = passwordEncryptor;
            _masterRepository = masterRepository;
        }

        public async Task<AuthenticationResponse> AuthenticateUser(LoginDetails loginDetails)
        {
            try
            {
                AuthenticationResponse authResponse = new AuthenticationResponse();
                List<string> MenuItemList = new List<string>();
                string MenuItemNames = "";
                string isChangePasswordRequired = "No";
                var user = this._dbContext.Users.FirstOrDefault(x => x.UserCode == loginDetails.UserName || x.Email == loginDetails.UserName && x.IsActive);

                if (user != null)
                {
                    if (user.IsLocked)
                    {
                        throw new Exception("User Account Locked! Please contact the Admin.");
                    }
                    else
                    {
                        var userRoles = await (from role in _dbContext.Roles
                                               join userRole in _dbContext.UserRoleMaps on role.RoleID equals userRole.RoleID
                                               join users in _dbContext.Users on userRole.UserID equals users.UserID
                                               where userRole.UserID == user.UserID && role.IsActive == true && userRole.IsActive == true
                                               select role).FirstOrDefaultAsync();

                        bool isValidUser = false;
                        string DecryptedPassword = _passwordEncryptor.Decrypt(user.Password, true);
                        isValidUser = DecryptedPassword == loginDetails.Password;

                        if (!isValidUser)
                        {
                            throw new Exception("Username or password is wrong");
                        }
                        if (_passwordEncryptor.IsPasswordChangeRequired(user.LastPasswordChangeDate))
                        {
                            isChangePasswordRequired = "Yes";
                        }
                        await this._masterRepository.LoginHistory(user.UserID, user.UserCode, user.UserName);
                        var Plants = _dbContext.UserPlantMaps.Where(x => x.UserID == user.UserID).Select(y => y.PlantCode).ToList();
                        if (userRoles != null)
                        {
                            MenuItemList = (from tb1 in _dbContext.Apps
                                            join tb2 in _dbContext.RoleAppMaps on tb1.AppID equals tb2.AppID
                                            where tb2.RoleID == userRoles.RoleID && tb1.IsActive == true && tb2.IsActive == true
                                            select tb1.AppName).ToList();
                            foreach (string item in MenuItemList)
                            {
                                if (MenuItemNames == "")
                                {
                                    MenuItemNames = item;
                                }
                                else
                                {
                                    MenuItemNames += "," + item;
                                }
                            }
                        }

                        authResponse.UserID = user.UserID;
                        authResponse.UserName = user.UserName;
                        authResponse.Email = user.Email;
                        authResponse.ContactNumber = user.ContactNumber;
                        authResponse.Role = userRoles.RoleName;
                        authResponse.MenuItemLists = MenuItemList;
                        authResponse.Plants = Plants;
                        authResponse.UserCode = user.UserCode;
                        authResponse.Role_Id = userRoles.RoleID;
                        authResponse.Token = GenerateToken(authResponse);
                        return authResponse;
                    }
                }
                else
                {
                    throw new Exception("Username or password is wrong");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateToken(AuthenticationResponse authenticationResponse)
        {
            string securityKey = _jwtSetting.securityKey;
            string issuer = _jwtSetting.issuer;
            string audience = _jwtSetting.audience;

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authenticationResponse.UserName),
                new Claim(ClaimTypes.Role, authenticationResponse.Role)
            };
            if (authenticationResponse.UserID != null && authenticationResponse.Role == "Admin")
            {
                claims.Add(new Claim("RoleId", authenticationResponse.Role_Id.ToString()));
            }

            if (authenticationResponse.UserID != null && authenticationResponse.Role == "Amararaja User")
            {
                claims.Add(new Claim("RoleId", authenticationResponse.Role_Id.ToString()));
            }

            if (authenticationResponse.UserID != null && authenticationResponse.Role == "Customer")
            {
                claims.Add(new Claim("UserId", authenticationResponse.UserID.ToString()));
            }

            var token = new JwtSecurityToken(
                                issuer: issuer,
                                audience: audience,
                                expires: DateTime.Now.AddHours(4),
                                signingCredentials: signingCredentials,
                                claims: claims
                            );
            var authToken = new JwtSecurityTokenHandler().WriteToken(token);
            return authToken;
        }

        public async Task<bool> ForgotPassword(ForgotPassword forgotPassword)
        {
            string[] decryptedArray = new string[3];
            string result = string.Empty;
            try
            {
                try
                {
                    result = _passwordEncryptor.Decrypt(forgotPassword.Token, true);
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
                    if (DateTime.Now > date)
                    {
                        throw new Exception("Reset password link expired!");
                    }
                    var DecryptedUserID = decryptedArray[0];

                    User user = (from tb in _dbContext.Users
                                 where tb.UserID.ToString() == DecryptedUserID && tb.IsActive
                                 select tb).FirstOrDefault();

                    if (user.UserName == decryptedArray[1] && forgotPassword.UserID == user.UserID)
                    {
                        try
                        {
                            TokenHistory history = _dbContext.TokenHistories.Where(x => x.UserID == user.UserID && !x.IsUsed && x.Token == forgotPassword.Token).Select(r => r).FirstOrDefault();
                            if (history != null)
                            {
                                user.FourthLastPassword = user.ThirdLastPassword;
                                user.ThirdLastPassword = user.SecondLastPassword;
                                user.SecondLastPassword = user.LastPassword;
                                user.LastPassword = user.Password;

                                user.Password = _passwordEncryptor.Encrypt(forgotPassword.NewPassword, true);
                                user.IsActive = true;
                                user.ModifiedOn = DateTime.Now;
                                await _dbContext.SaveChangesAsync();

                                history.UsedOn = DateTime.Now;
                                history.IsUsed = true;
                                history.Comment = "Token Used successfully";
                                await _dbContext.SaveChangesAsync();
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

        public async Task<bool> ChangePassword(ForgotPasswordOTP forgotPasswordOTP)
        {
            string result = string.Empty;
            try
            {
                User user = (from tb in _dbContext.Users
                             where tb.UserCode == forgotPasswordOTP.UserCode && tb.IsActive
                             select tb).FirstOrDefault();

                if (user != null)
                {
                    try
                    {
                        TokenHistory history = _dbContext.TokenHistories.Where(x => x.UserID == user.UserID && x.OTP == forgotPasswordOTP.OTP).Select(r => r).FirstOrDefault();
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
                                    user.Password = _passwordEncryptor.Encrypt(forgotPasswordOTP.NewPassword, true);
                                    user.IsActive = true;
                                    user.ModifiedOn = DateTime.Now;
                                    await _dbContext.SaveChangesAsync();

                                    history.UsedOn = DateTime.Now;
                                    history.IsUsed = true;
                                    history.Comment = "OTP Used successfully";
                                    await _dbContext.SaveChangesAsync();
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
    }
}
