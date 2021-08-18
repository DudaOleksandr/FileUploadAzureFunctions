using System.Collections.Generic;
using FileUploadMonitor.Core.Dtos;


namespace FileUploadMonitor.Core.Interfaces
{
    public interface IFileParser
    {
       public IEnumerable<TransactionDto> ParseFile(string fileBody, string fileName); 
    }
}
