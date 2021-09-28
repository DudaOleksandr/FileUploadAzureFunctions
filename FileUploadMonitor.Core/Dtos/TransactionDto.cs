using System;
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

        protected bool Equals(TransactionDto other)
        {
            return TransactionId == other.TransactionId && Amount == other.Amount && CurrencyCode == other.CurrencyCode && TransactionDate.Equals(other.TransactionDate) && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is not TransactionDto objDto)
            {
                return false;
            }

            return Equals(objDto);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TransactionId, Amount, CurrencyCode, TransactionDate, Status);
        }
    }
}
