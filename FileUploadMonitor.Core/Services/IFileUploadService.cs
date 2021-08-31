using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;

namespace FileUploadMonitor.Core.Services
{
    public interface IFileUploadService
    {
       public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName);

       public Task<List<TransactionDto>> ParseTransaction(string transactionBody);

        public Task<List<OutputTransactionDto>>GetTransactions(string currency, string status, string dateFrom, string dateTo);

       public void SaveFile(IEnumerable<TransactionDto> transactions);
    }
}
