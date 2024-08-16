
using AREML.EPOD.Core.Entities;
using AREML.EPOD.Data.Helpers;
using AREML.EPOD.Data.Repositories;
using AREML.EPOD.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddScoped<IMasterRepository,MasterRepository>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddSingleton<PasswordEncryptor>();

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
