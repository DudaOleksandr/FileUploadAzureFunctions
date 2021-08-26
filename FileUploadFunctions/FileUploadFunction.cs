using System;
using System.Collections.Generic;
using System.Net;
using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;

namespace FileUploadFunctions
{
    public static class FileUploadFunction
    {
        private static readonly CloudStorageAccount CloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=parserfileupload;AccountKey=Ge66MVT4eijCK5xSFUa7D/LdjsrDpxofy+8xBfkf26+fqOdjUNTOIEX/i+A6h3yU4qbZ27EXb4TO+GGfoTISSA==;EndpointSuffix=core.windows.net");


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
