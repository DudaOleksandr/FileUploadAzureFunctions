using System;
using System.Collections.Generic;
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

        public FileTriggerFunction(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [Function("FileTriggerFunction")]
        [ServiceBusOutput("fileupload", Connection = "ServiceBusConnectionWrite")]
        public IEnumerable<string> Run(
            [BlobTrigger("file-storage/{name}", Connection = "ConnectionString")] string  myBlob, string name,
            FunctionContext context)

        { 
            var logger = context.GetLogger("FileTriggerFunction");
            try
            {
                var res = _fileUploadService.ParseFile(myBlob, name);
                return res;
            }
            catch (ValidationAggregationException validationAggregationException)
            {
                logger.LogInformation(JsonConvert.SerializeObject(validationAggregationException.Message, Formatting.Indented));

                foreach (var error in validationAggregationException.Exceptions)
                {
                    logger.LogInformation(JsonConvert.SerializeObject(new {error.Message, error.Source}, Formatting.Indented));
                }

                throw;
            }
            catch (Exception ex)
            {
                logger.LogInformation(JsonConvert.SerializeObject(new {ex.Message, ex.Source }, Formatting.Indented));
                throw;
            }

            
        }
    }
}
