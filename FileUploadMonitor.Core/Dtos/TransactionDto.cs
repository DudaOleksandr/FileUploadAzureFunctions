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
        public DateTime? TransactionDate { get; set; }
        
        [JsonProperty("status")]
        public StatusType? Status { get; set; }
    }
}
