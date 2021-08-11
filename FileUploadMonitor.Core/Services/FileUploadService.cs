using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using FileUploadMonitor.Core.Parsers;


namespace FileUploadMonitor.Core.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly int _maxFileSizeMb;

        private readonly ITransactionsService _transactionsService;

        public FileUploadService(IConfiguration config, ITransactionsService transactionsService)
        {
            _transactionsService = transactionsService;
            _maxFileSizeMb = config.GetValue<int>("MaxFileSizeMb");
        }

        public IEnumerable<TransactionDto> UploadFile(IFormFile file)
        {
            if (!IsFileSizeValid(file))
            {
                throw new Exception("File size is invalid");
            }
            var parser = GetFileParser(file);

            return _transactionsService.Save(parser.ParseFile(file).ToList());
        }

        public Task<List<OutputTransactionDto>> GetTransactions(string currency, string status, string dateFrom, string dateTo)
        {
            return _transactionsService.Get(currency,status,dateFrom,dateTo);
        }

        private static IFileParser GetFileParser(IFormFile file)
        {
            var type = file.ContentType.Split('/').Last();
            if (Enum.TryParse(type, true, out FileType fileType))
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

        private bool IsFileSizeValid(IFormFile file)
        {
            return file.Length < _maxFileSizeMb * 1048576 && file.Length > 0;
        }
    }
}
