using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FileUploadFunctions
{
    public static class FileTriggerFunction
    {
        [Function("FileTriggerFunction")]
        public static void Run([BlobTrigger("file-storage/{name}", Connection = "ConnectionString")] string myBlob, string name,
            FunctionContext context)
        {
            var logger = context.GetLogger("FileTriggerFunction");
            logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {myBlob}");
        }
    }
}
