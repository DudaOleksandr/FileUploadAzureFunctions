using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;

namespace FileUploadMonitor.Core.Services
{
    public interface ITransactionsService
    {
        public IEnumerable<TransactionDto> Save(List<TransactionDto> transactionModels);

        public Task<List<OutputTransactionDto>> Get(string currency, string status, string dateFrom, string dateTo);

    }
}
