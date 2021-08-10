using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace HttpTriggerToFileUpload
{
    public static class FileTriggerFunction
    {
        [FunctionName("FileTriggerFunction")]
        public static void Run([BlobTrigger("file-storage/{name}", Connection = "ConnectionString")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
