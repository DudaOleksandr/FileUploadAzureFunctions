using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Common.Exceptions;
using FileUploadMonitor.Core.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FileUploadFunctions
{
    public class FileTriggerFunction
    {

        private readonly IFileUploadService _fileUploadService;

        private static readonly string ConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionWrite");

        private static readonly string QueueName = "fileupload";

        private static ServiceBusClient _client;

        private static ServiceBusSender _sender;

        public FileTriggerFunction(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [Function("FileTriggerFunction")]
        public async Task Run(
            [BlobTrigger("file-storage/{name}", Connection = "AzureConnectionString")] string fileContent, string name,
            FunctionContext context)

        {
            var logger = context.GetLogger("FileTriggerFunction");
            _client = new ServiceBusClient(ConnectionString);
            _sender = _client.CreateSender(QueueName);
            try
            {
                var res = _fileUploadService.ParseFile(fileContent, name).ToList();
                foreach (var message in res)
                {
                    await _sender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(message, Formatting.Indented)));
                }
            }
            catch (ValidationAggregationException validationAggregationException)
            {
                logger.LogInformation(validationAggregationException.Message);

                foreach (var error in validationAggregationException.Exceptions)
                {
                    logger.LogInformation(error.Message);
                }
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message);
            }
            finally
            {
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }
        }
    }
}
