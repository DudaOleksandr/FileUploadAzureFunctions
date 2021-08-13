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
        public void Run(
            [BlobTrigger("file-storage/{name}", Connection = "ConnectionString")] string  myBlob, string name,
            FunctionContext context)
        {
            var res = _fileUploadService.UploadFile(myBlob, name);
            var logger = context.GetLogger("FileTriggerFunction");
            logger.LogInformation(JsonConvert.SerializeObject(res, Formatting.Indented));
        }
    }
}
