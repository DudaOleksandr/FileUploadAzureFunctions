using System.Threading.Tasks;
using FileUploadMonitor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileUploadFunctions
{
    public class ServiceBusTriggerFunction
    {
        private readonly IFileUploadService _fileUploadService;

        public ServiceBusTriggerFunction(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [Function("ServiceBusTriggerFunction")]
        public async Task Run([ServiceBusTrigger("fileupload", Connection = "ServiceBusConnectionRead")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger("ServiceBusTriggerFunction");
            var res = await _fileUploadService.ParseTransaction(myQueueItem);
            logger.LogInformation($"\n \n New Message received: \n {myQueueItem} \n \n \n transaction output: \n {JsonConvert.SerializeObject(res)}");
        }
    }
}
