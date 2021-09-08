using System.Collections.Generic;
using System.Linq;
using FileUploadMonitor.Core.Dtos;

namespace FileUploadMonitor.Core.Services
{
    public class TransactionParseService : ITransactionParseService
    {
        private readonly IBlobService _blobService;
        
        private readonly IFileUploadService _fileUploadService;

        private readonly ITransactionsService _transactionsService;
        
        public TransactionParseService(IBlobService blobService, IFileUploadService fileUploadService, ITransactionsService transactionsService)
        {
            _blobService = blobService;
            _fileUploadService = fileUploadService;
            _transactionsService = transactionsService;
        }
        
        public List<TransactionDto> ParseTransaction(TransactionBatchEventDto transactionBody)
        {
            var fileBody = _blobService.GetBlob(transactionBody.FileName);
            var parser = _fileUploadService.GetFileParser(transactionBody.FileName);
            var res = parser.ParseTransaction(transactionBody.From, transactionBody.To, fileBody.Result).ToList();
            _transactionsService.Save(res);
            return res;
        }
    }
}