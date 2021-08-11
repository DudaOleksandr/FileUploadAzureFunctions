using System.Collections.Generic;
using FileUploadMonitor.Core.Dtos;
using Microsoft.AspNetCore.Http;


namespace FileUploadMonitor.Core.Interfaces
{
    public interface IFileParser
    {
       public IEnumerable<TransactionDto> ParseFile(IFormFile file); 
    }
}
