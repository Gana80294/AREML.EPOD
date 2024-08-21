using AREML.EPOD.API.Extensions;
using AREML.EPOD.Core.Configurations;
using AREML.EPOD.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using AREML.EPOD.API.Auth;
using AREML.EPOD.Data.Filters;

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
            builder.Services.AddCors(options => { options.AddPolicy("cors", a => a.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()); });

            builder.Services.AddDbContext<AuthContext>(opts => opts.UseSqlServer(builder.Configuration.GetConnectionString("DBContext")));
            

            // Add Configuration Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWTSecurity"));

            // Add Authentication

            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            //}).AddJwtBearer(jwt =>
            //{
            //    jwt.RequireHttpsMetadata = false;
            //    jwt.SaveToken = true;
            //    jwt.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        RequireExpirationTime = true,
            //        ValidateLifetime = true,
            //        RequireSignedTokens = true,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWTSecurity").GetValue<string>("securityKey"))),
            //        ValidateIssuer = true,
            //        ValidIssuer = builder.Configuration.GetSection("JWTSecurity").GetValue<string>("issuer"),
            //        ValidateAudience = true,
            //        ValidAudience = builder.Configuration.GetSection("JWTSecurity").GetValue<string>("audience")
            //    };
            //});

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<AuthMiddleware>();

            //app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
