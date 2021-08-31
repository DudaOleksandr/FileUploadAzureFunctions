using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Common.Enums;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using FileUploadMonitor.Core.Parsers;


namespace FileUploadMonitor.Core.Services
{
    public class FileUploadService : IFileUploadService
    {
        private const int MaxFileSizeMb = 1;

        private readonly ITransactionsService _transactionsService;

        public FileUploadService(ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
        }

        public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName)
        {
            if (!IsFileSizeValid(fileBody))
            {
                throw new ValidationException("File size is invalid", nameof(fileBody));
            }
            var parser = GetFileParser(fileName);

            return parser.ParseFile(fileBody, fileName).ToList();
        }

        public async Task<List<TransactionDto>> ParseTransaction(string transactionBody)
        {
            var transactionInfo = transactionBody.Split(",");
            var fileBody = GetBlob( transactionInfo.Last());
            var parser = GetFileParser(transactionInfo.Last());
            var res = parser.ParseTransaction(transactionInfo.First(), fileBody.Result);
            var transactionDtos = res.ToList();
             await _transactionsService.Save(transactionDtos.ToList());

            return transactionDtos.ToList();
        }


        public Task<List<OutputTransactionDto>> GetTransactions(string currency, string status, string dateFrom, string dateTo)
        {
            return _transactionsService.Get(currency,status,dateFrom,dateTo);
        }

        public void SaveFile(IEnumerable<TransactionDto> transactions)
        {
            _transactionsService.Save(transactions.ToList());
        }

        private static IFileParser GetFileParser(string fileName)
        {
            var rr = fileName.Split('.').Last();
            if (Enum.TryParse(rr, true, out FileType fileType))
            {
                switch (fileType)
                {
                    case FileType.Csv:
                        return new CsvParser();
                    case FileType.Json:
                        return new JsonParser();
                    case FileType.Xml:
                        return new XmlParser();
                }
            }
            throw new ValidationException("Unknown format", nameof(fileType));
        }

        private bool IsFileSizeValid(string file)
        {
            return file.Length is < MaxFileSizeMb * 1048576 and > 0;
        }

        private async Task<string> GetBlob(string fileName)
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
