﻿using System;
using System.Collections.Generic;
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

        // name of your Service Bus queue
        private static readonly string QueueName = "fileupload";

        // the client that owns the connection and can be used to create senders and receivers
        private static ServiceBusClient _client;

        // the sender used to publish messages to the queue
        private static ServiceBusSender _sender;

        public FileTriggerFunction(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [Function("FileTriggerFunction")]
        [ServiceBusOutput("fileupload", Connection = "ServiceBusConnectionWrite")]
        public async Task<IEnumerable<string>> Run(
            [BlobTrigger("file-storage/{name}", Connection = "ConnectionString")] string  myBlob, string name,
            FunctionContext context)

        {
            var logger = context.GetLogger("FileTriggerFunction");

            _client = new ServiceBusClient(ConnectionString);
            _sender = _client.CreateSender(QueueName);
            using var messageBatch = await _sender.CreateMessageBatchAsync();
           
            var res = _fileUploadService.ParseFile(myBlob, name).ToList();
            foreach (var message in res)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage(message)))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {message} is too large to fit in the batch.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await _sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {res.Count} messages has been published to the queue.");
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
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }
            return res;
        }
    }
}
