using AREML.EPOD.API.Extensions;
using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using NPOI.SS.Formula.Functions;
using AutoMapper;
using AREML.EPOD.Data.Mapper;
using Newtonsoft.Json.Serialization;
using AREML.EPOD.API.Auth;
using AREML.EPOD.Data.Filters;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Data.Logging;

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

            var corsUrls = builder.Configuration["Cors"]?.Split(",") ?? Array.Empty<string>();
            LogWriter.WriteToFile(corsUrls.ToString());
            builder.Services.AddCors(options => { options.AddPolicy("cors", a => a.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
            //if (corsUrls.Length > 0) 
            //{
            //    builder.Services.AddCors(options => { options.AddPolicy("cors", a => a.WithOrigins(corsUrls).AllowAnyHeader().AllowAnyMethod()); });
            //}
            //else
            //{
            //    builder.Services.AddCors(options => { options.AddPolicy("cors", a => a.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); });
            //}

            builder.Services.AddDbContext<AuthContext>(opts => 
            opts.UseSqlServer(builder.Configuration.GetConnectionString("DBContext"), 
            
            sqlOptions =>
            {
                sqlOptions.CommandTimeout(180);
            }).LogTo(query => { LogWriter.WriteSensitiveLog(query); }), ServiceLifetime.Scoped);


            // Add Configuration Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWTSecurity"));

            builder.Services.AddControllers(config => { config.Filters.Add(typeof(CommonExceptionFilter)); }).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            builder.Services.AddRepositories();
            builder.Services.AddPolicies();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddMvc(config =>
            {
                config.Filters.Add<CommonExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                // Use the default property (Pascal) ca sing.
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });
            builder.Services.AddSingleton<PasswordEncryptor>();
            builder.Services.AddSingleton<ExcelHelper>();
            builder.Services.AddSingleton<PdfCompresser>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<MasterProfile>();
            builder.Services.AddSingleton<EmailHelper>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseCors("cors");

            app.UseHttpsRedirection();

            //app.UseMiddleware<AuthMiddleware>();

            //app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("cors");

            app.MapControllers();

            app.Run();
        }
    }
}
