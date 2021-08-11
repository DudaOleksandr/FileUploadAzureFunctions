using System;
using FileUploadMonitor.Domain.Interfaces;

namespace FileUploadMonitor.Domain.Entities
{
    public class Transaction : IDbEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public virtual DateTime TransactionDate { get; set; }
        public string Status { get; set; }

    }
}
