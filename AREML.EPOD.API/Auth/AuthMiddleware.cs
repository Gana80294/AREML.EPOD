using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AREML.EPOD.API.Auth
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSetting _jwtSetting;

        public AuthMiddleware(RequestDelegate next, IOptions<JwtSetting> setting)
        {
            _next = next;
            _jwtSetting = setting.Value;
        }

        public async Task Invoke(HttpContext httpContext, AuthContext context)
        {
            var path = httpContext.Request.Path;
            var endPoint = path.Value!.ToString().ToLower();
            if (endPoint.Contains("authenticateuser") ||
                endPoint.Contains("authenticateuser"))
            {
                await _next(httpContext);
            }
            else
            {
                try
                {
                    string token = string.Empty;
                    string issuer = _jwtSetting.issuer; //Get issuer value from your configuration
                    string audience = _jwtSetting.audience; //Get audience value from your configuration
                    SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.securityKey));

                    CustomAuthHandler authHandler = new CustomAuthHandler(context);
                    var header = httpContext.Request.Headers["Authorization"];
                    if (header.Count == 0) throw new Exception("Authorization header is empty");
                    string[] tokenValue = Convert.ToString(header).Trim().Split(" ");
                    if (tokenValue.Length > 1) token = tokenValue[1];
                    else throw new Exception("Authorization token is empty");
                    if (authHandler.IsValidToken(token, issuer, audience, signingKey))
                        await _next(httpContext);

                }
                catch (Exception)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    HttpResponseWritingExtensions.WriteAsync(httpContext.Response, "{\"message\": \"Unauthorized\"}").Wait();
                }
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
