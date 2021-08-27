using FileUploadMonitor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
        public void Run([ServiceBusTrigger("fileupload", Connection = "ServiceBusConnectionRead")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger("ServiceBusTriggerFunction");
            var res = _fileUploadService.ParseTransaction(myQueueItem);
            logger.LogInformation($"\n \n New Message received: \n {myQueueItem} \n");
            logger.LogInformation($"\n \n transactionOutput: \n {res}");

        }
    }
}
