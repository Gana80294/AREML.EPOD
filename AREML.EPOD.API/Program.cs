using AREML.EPOD.API.Extensions;
using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace AREML.EPOD.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options => { options.AddPolicy("cors", a => a.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });

            builder.Services.AddDbContext<AuthContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DBContext")));
            builder.Services.AddSingleton<PasswordEncryptor>();

            // Add Configuration Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWTSecurity"));

            // Add Authentication

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWTSecurity").GetValue<string>("securityKey"))),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetSection("JWTSecurity").GetValue<string>("issuer"),
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetSection("JWTSecurity").GetValue<string>("audience")
                };
            });

            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            builder.Services.AddRepositories();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSingleton<PasswordEncryptor>();
            builder.Services.AddSingleton<ExcelHelper>();
            builder.Services.AddSingleton<PdfCompresser>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
