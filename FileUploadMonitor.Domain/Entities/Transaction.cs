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

        protected bool Equals(Transaction other)
        {
            return Id == other.Id && Amount == other.Amount && CurrencyCode == other.CurrencyCode && TransactionDate.Equals(other.TransactionDate) && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Transaction)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Amount, CurrencyCode, TransactionDate, Status);
        }
    }
}
