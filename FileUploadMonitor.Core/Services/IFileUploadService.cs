using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;

namespace FileUploadMonitor.Core.Services
{
    public interface IFileUploadService
    {
       public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName);
       
       public Task<List<OutputTransactionDto>>GetTransactions(string currency, string status, string dateFrom, string dateTo);

       public void SaveFile(IEnumerable<TransactionDto> transactions);

       public  IFileParser GetFileParser(string fileName);

    }
}
