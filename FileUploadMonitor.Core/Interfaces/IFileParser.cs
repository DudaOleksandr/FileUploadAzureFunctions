using System.Collections.Generic;
using FileUploadMonitor.Core.Dtos;


namespace FileUploadMonitor.Core.Interfaces
{
    public interface IFileParser
    {
       public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName);

       public IEnumerable<TransactionDto> ParseTransaction(int from, int to, string fileBody);
    }
}
