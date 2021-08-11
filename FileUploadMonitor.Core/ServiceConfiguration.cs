using FileUploadMonitor.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileUploadMonitor.Core
{
    public static class ServiceConfiguration
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IFileUploadService, FileUploadService>();

            services.AddTransient<ITransactionsService, TransactionsService>();
        }
    }
}
