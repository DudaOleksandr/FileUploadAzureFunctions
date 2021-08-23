using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileUploadFunctions
{
    public static class ServiceBusTriggerFunction
    {
        [Function("ServiceBusTriggerFunction")]
        public static void Run([ServiceBusTrigger("fileupload", Connection = "ServiceBusConnectionRead")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger("ServiceBusTriggerFunction");
            logger.LogInformation(myQueueItem);
        }
    }
}
