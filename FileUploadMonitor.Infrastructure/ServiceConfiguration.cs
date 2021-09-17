using FileUploadMonitor.Core;
using FileUploadMonitor.Domain.Interfaces;
using FileUploadMonitor.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace FileUploadMonitor.Infrastructure
{
    public static class ServiceConfiguration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddDbContext<ApplicationDbContext>();

            services.AddCoreServices();
        }
    }
}