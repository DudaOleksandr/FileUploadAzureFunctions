using System.Collections.Generic;
using FileUploadMonitor.Core.Dtos;


namespace FileUploadMonitor.Core.Interfaces
{
    public interface IFileParser
    {
       public IEnumerable<string> ParseFile(string fileBody, string fileName);

       public TransactionDto ParseTransaction(string transactionBody);
    }
}
