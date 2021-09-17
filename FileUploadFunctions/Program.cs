using Common.Middleware;
using FileUploadMonitor.Infrastructure;
using Microsoft.Extensions.Hosting;

namespace FileUploadFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services =>
                {
                    services.AddInfrastructureServices();
                })
                .Build();

            host.Run();
        }
    }
}