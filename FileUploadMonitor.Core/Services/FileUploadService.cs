using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;
using Common.Exceptions;
using Microsoft.Extensions.Configuration;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using FileUploadMonitor.Core.Parsers;
using MimeMapping;


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

        public IEnumerable<TransactionDto> UploadFile(string fileBody, string fileName)
        {
            /*if (!IsFileSizeValid(fileBody))
            {
                throw new ValidationException("File size is invalid", nameof(fileBody));
            }*/
            var parser = GetFileParser(fileName);

            return _transactionsService.Save(parser.ParseFile(fileBody, fileName).ToList());
        }

        public Task<List<OutputTransactionDto>> GetTransactions(string currency, string status, string dateFrom, string dateTo)
        {
            return _transactionsService.Get(currency,status,dateFrom,dateTo);
        }

        private static IFileParser GetFileParser(string fileName)
        {
            var rr = fileName.Split('.').Last();
            var contentType = MimeUtility.GetExtensions(rr);
            //var type = contentType[0].Split('/').Last();
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
            return System.Text.Encoding.Unicode.GetByteCount(file) < _maxFileSizeMb * 1048576 && file.Length > 0;
        }
    }
}
