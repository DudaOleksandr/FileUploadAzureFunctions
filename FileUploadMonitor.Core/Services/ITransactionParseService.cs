using System.Collections.Generic;
using FileUploadMonitor.Core.Dtos;

namespace FileUploadMonitor.Core.Services
{
    public interface ITransactionParseService
    {
        public List<TransactionDto> ParseTransaction(TransactionBatchEventDto transactionBody);

    }
}