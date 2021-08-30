using System;
using System.Net;
using HttpMultipartParser;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace FileUploadFunctions
{
    public static class FileUploadFunction
    {
        private static readonly CloudStorageAccount CloudStorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureConnectionString"));

        [Function("FileUploadFunction")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            try
            {
                var parsedFormBody = MultipartFormDataParser.ParseAsync(req.Body);
                var file = parsedFormBody.Result.Files[0];
                var cloudBlobClient = CloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference("file-storage");
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.ContentType.Split(@"/")[^1] + @"\"
                    + file.FileName.Split(@"\")[^1]);

                cloudBlockBlob.UploadFromStreamAsync(file.Data);

                
                var okResponse = req.CreateResponse(HttpStatusCode.OK);
                okResponse.WriteAsJsonAsync(file.FileName);
                return okResponse;

            }
            catch (Exception ex)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);

                badResponse.WriteAsJsonAsync(ex);

                return badResponse;
            }
        }
    }
}
