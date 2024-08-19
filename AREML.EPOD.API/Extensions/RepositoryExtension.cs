using AREML.EPOD.Data.Repositories;
using AREML.EPOD.Interfaces.IRepositories;

namespace AREML.EPOD.API.Extensions
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IDataMigrationRepository,DataMigrationRepository>();
            services.AddScoped<IForwardLogisticsRepository, ForwardLogisticsRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IMasterRepository, MasterRepository>();
            services.AddScoped<IMobileRepository, MobileRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IReverseLogisticsRepository, ReverseLogisticsRepository>();
            services.AddScoped<IAuthenticationRepository,AuthenticationRepository>();

            return services;
        }
    }
}
