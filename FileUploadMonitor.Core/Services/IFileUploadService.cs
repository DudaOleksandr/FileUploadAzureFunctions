using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;
using Microsoft.AspNetCore.Http;

namespace FileUploadMonitor.Core.Services
{
    public interface IFileUploadService
    {
       public IEnumerable<TransactionDto> UploadFile(string fileBody, string fileName);
       public Task<List<OutputTransactionDto>>GetTransactions(string currency, string status, string dateFrom, string dateTo);
    }
}
