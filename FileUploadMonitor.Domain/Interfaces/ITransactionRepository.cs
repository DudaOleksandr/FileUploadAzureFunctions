using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileUploadMonitor.Domain.Entities;

namespace FileUploadMonitor.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAll(string currency, string status, DateTime? dateFrom, DateTime? dateTo);

        void Add(Transaction entity);

        void AddRange(IEnumerable<Transaction> entities);

        void Delete(Transaction entity);
        
        Transaction GetById(string id);
    }
}
