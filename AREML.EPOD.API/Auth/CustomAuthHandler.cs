using AREML.EPOD.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AREML.EPOD.API.Auth
{
    public class CustomAuthHandler
    {
        private readonly AuthContext _dbContext;
        public CustomAuthHandler(AuthContext context)
        {
            _dbContext = context;
        }

        public bool IsValidToken(string jwtToken, string issuer, string audience, SymmetricSecurityKey signingKey)
        {
            return ValidateToken(jwtToken, issuer, audience, signingKey);
        }

        private bool ValidateToken(string jwtToken, string issuer, string audience, SymmetricSecurityKey signingKey)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience
                };
                ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();
                var claim = tokenValidator.ValidateToken(jwtToken, validationParameters, out var _);
                var access = ValidatePermission(jwtToken);
                return true;
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException();
            }
        }

        private bool ValidatePermission(string jwtToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtTokenObj = tokenHandler.ReadJwtToken(jwtToken);


                var role = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;
                if (role != "Vendor")
                {
                    string userCode = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == "UserCode")!.Value;
                    if (!string.IsNullOrWhiteSpace(userCode) && !string.IsNullOrWhiteSpace(role))
                    {
                        var user = this._dbContext.Users.FirstOrDefault(x => x.UserCode == userCode);
                        if (user != null) { return true; }
                    }
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    string vendorCode = jwtTokenObj.Claims.FirstOrDefault(c => c.Type == "VendorCode")!.Value;
                    if (!string.IsNullOrWhiteSpace(vendorCode) && !string.IsNullOrWhiteSpace(role))
                    {
                        if (vendorCode == "not_registered")
                        {
                            return true;
                        }
                        var user = this._dbContext.Users.FirstOrDefault(x => x.UserCode == vendorCode);
                        if (user != null) { return true; }
                    }
                    throw new UnauthorizedAccessException();
                }
                return true;
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
