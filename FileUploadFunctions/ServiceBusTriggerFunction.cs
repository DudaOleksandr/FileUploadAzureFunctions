using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileUploadFunctions
{
    public class ServiceBusTriggerFunction
    {
        private readonly ITransactionParseService _transactionParseService;

        public ServiceBusTriggerFunction(ITransactionParseService transactionsService)
        {
            _transactionParseService = transactionsService;
        }

        [Function("ServiceBusTriggerFunction")]
        public async Task Run([ServiceBusTrigger("fileupload", Connection = "ServiceBusConnectionRead")] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger("ServiceBusTriggerFunction");
            var deserializedMessages = JsonConvert.DeserializeObject<TransactionBatchEventDto>(myQueueItem);
            var res =  _transactionParseService.ParseTransaction(deserializedMessages);
            logger.LogInformation($"\n \n New Message received: \n {myQueueItem} \n \n \n transaction output: \n {JsonConvert.SerializeObject(res)}");
        }
    }
}
