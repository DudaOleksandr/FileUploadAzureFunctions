using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Common.Exceptions;

namespace FileUploadMonitor.Core.Services
{
    public class BlobService : IBlobService
    {
        public async Task<string> GetBlob(string fileName)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureConnectionString");
            var containerName = "file-storage";
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName.Replace(" ",""));
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using var streamReader = new StreamReader(response.Value.Content);
                return await streamReader.ReadToEndAsync();
            }

            throw new ValidationException("Blob does not exist", nameof(blobClient));
        }
    }
}