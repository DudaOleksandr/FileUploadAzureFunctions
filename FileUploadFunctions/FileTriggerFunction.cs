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
            [BlobTrigger("file-storage/{name}", Connection = "AzureConnectionString")] string  myBlob, string name,
            FunctionContext context)

        {
            var logger = context.GetLogger("FileTriggerFunction");

            _client = new ServiceBusClient(ConnectionString);
            _sender = _client.CreateSender(QueueName);
            try
            {
                var res = _fileUploadService.ParseFile(myBlob, name).ToList();
                foreach (var message in res)
                {
                    await _sender.SendMessageAsync(new ServiceBusMessage($"{message.From}-{message.To},{message.FileName}"));
                }
            }
            catch (ValidationAggregationException validationAggregationException)
            {
                logger.LogInformation(JsonConvert.SerializeObject(validationAggregationException.Message, Formatting.Indented));

                foreach (var error in validationAggregationException.Exceptions)
                {
                    logger.LogInformation(JsonConvert.SerializeObject(new { error.Message, error.Source }, Formatting.Indented));
                }

                throw;
            }
            catch (Exception ex)
            {
                logger.LogInformation(JsonConvert.SerializeObject(new { ex.Message, ex.Source }, Formatting.Indented));
                throw;
            }
            finally
            {
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }
        }
    }
}
