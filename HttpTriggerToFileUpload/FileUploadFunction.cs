using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using MimeMapping;


namespace HttpTriggerToFileUpload
{
    public static class FileUploadFunction
    {
        private static readonly CloudStorageAccount CloudStorageAccount = CloudStorageAccount
            .Parse("ConnectionString");


        [FunctionName("FileUploadFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var formData = await req.ReadFormAsync();
                var file = req.Form.Files["file"];
                var cloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference("file-storage");

                var contentType = MimeUtility.GetMimeMapping(file.FileName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(contentType.Split(@"/")[^1] + @"\"
                    + file.FileName.Split(@"\")[^1]);
                cloudBlockBlob.Properties.ContentType = contentType;

                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());

                return new OkObjectResult(file.FileName + " - " + file.Length.ToString());
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
