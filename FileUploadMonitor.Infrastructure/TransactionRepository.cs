using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using FileUploadMonitor.Domain.Entities;
using FileUploadMonitor.Domain.Interfaces;
using FileUploadMonitor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileUploadMonitor.Infrastructure
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Transaction>> GetAll(string currency, string status, DateTime? dateFrom, DateTime? dateTo)
        {
            var transactions = _context.Transactions
                .WhereIf(currency is not null, t => t.CurrencyCode == currency)
                .WhereIf(status is not null, t => t.Status == status)
                .WhereIf(dateFrom is not null && dateTo is not null, t => dateFrom < t.TransactionDate && t.TransactionDate < dateTo)
                .AsNoTracking()
                .ToListAsync();
            
            return transactions;

        }

        public void Add(Transaction entity)
        {
            _context.Transactions.Add(entity);
            _context.SaveChanges();
        }

        public async Task AddRange(IEnumerable<Transaction> entities)
        {
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public void Delete(Transaction entity)
        {
            _context.Transactions.Remove(entity);
            _context.SaveChanges();
        }

        public Transaction GetById(string id)
        {
            return _context.Transactions.FirstOrDefault(e => e.Id == id);
        }

    }
}
