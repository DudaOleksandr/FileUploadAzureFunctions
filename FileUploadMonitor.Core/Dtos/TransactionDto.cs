using System;
using System.Collections.Generic;
using Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FileUploadMonitor.Core.Dtos
{
    public class TransactionDto
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("amount")]
        public decimal? Amount { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("transaction_date", ItemConverterType = typeof(JavaScriptDateTimeConverter))]
        public DateTime TransactionDate { get; set; }
        
        [JsonProperty("status")]
        public StatusType? Status { get; set; }
        
        /*public override bool Equals(object other)
        {
            // If the passed object is null
            if (other == null)
            {
                return false;
            }
            if (!(other is TransactionDto))
            {
                return false;
            }
            return TransactionId.Equals((TransactionDto)other.TransactionId)
                   && Amount.Equals(other.Amount)
                   && CurrencyCode.Equals(other.CurrencyCode) 
                   && TransactionDate.Equals(other.TransactionDate) 
                   && Status.Equals(other.Status);
            return (this.FirstName == ((Customer)obj).FirstName)
                   && (this.LastName == ((Customer)obj).LastName);
        }*/

        protected bool Equals(TransactionDto other)
        {
            return TransactionId == other.TransactionId && Amount == other.Amount && CurrencyCode == other.CurrencyCode && TransactionDate.Equals(other.TransactionDate) && Status == other.Status;
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

            if (obj.GetType() != typeof(TransactionDto))
            {
                return false;
            }

            return Equals((TransactionDto)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TransactionId, Amount, CurrencyCode, TransactionDate, Status);
        }
    }
}
