using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FileUploadFunctions
{
    public static class TestTriggerFunction
    {
        private static readonly string ConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionWrite");

        // name of your Service Bus queue
        private static readonly string QueueName = "fileupload";

        // the client that owns the connection and can be used to create senders and receivers
        private static ServiceBusClient _client;

        // the sender used to publish messages to the queue
        private static ServiceBusSender _sender;

        [Function("TestTriggerFunction")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _client = new ServiceBusClient(ConnectionString);
            _sender = _client.CreateSender(QueueName);
            using var messageBatch = await _sender.CreateMessageBatchAsync();

            var messagesList = new List<string>()
            {
                new("1 - 1, testFilename1.csv"),
                new("2 - 2, testFilename2.csv"),
                new("3 - 3, testFilename3.csv"),
                new("4 - 4, testFilename4.csv")
            };

            foreach (var message in messagesList)
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
                Console.WriteLine($"A batch of {messagesList.Count} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await _sender.DisposeAsync();
                await _client.DisposeAsync();
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}
