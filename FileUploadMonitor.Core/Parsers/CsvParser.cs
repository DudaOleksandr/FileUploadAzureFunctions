using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Common.Enums;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;

namespace FileUploadMonitor.Core.Parsers
{
    public class CsvParser : IFileParser
    {
        public IEnumerable<TransactionDto> ParseFile(string fileBody, string fileName)
        {
            var transactionsList = new List<TransactionDto>();
            var exceptionList = new List<ValidationException>();
            var fileSplits = fileBody.Split("\n");
            var transactionCounter = 0;
            foreach (var line in fileSplits)
            {
                var parserPattern = "\"([^\"]*)\"";
                var options = RegexOptions.Multiline;
                var splits = Regex.Matches(line, parserPattern, options).Select(x => x.Value.Replace("\"", "")).ToList();

                if (splits.Count != 5)
                {
                    throw new ValidationException("There is invalid file structure", nameof(splits));
                }
                var transaction = splits[0];

                if (!decimal.TryParse(splits[1], out var amount))
                {
                    exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(amount)}"));
                    continue;
                }
                var currencyCode = splits[2];

                if (!DateTime.TryParseExact(splits[3], "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dateTime))
                {
                    exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(dateTime)}"));
                    continue;
                }
                if (!Enum.TryParse(splits[4], true, out StatusType status))
                {
                    exceptionList.Add(new ValidationException("Invalid value", $"transaction.{transactionCounter}.{nameof(status)}"));
                    continue;
                }

                transactionsList.Add(new TransactionDto
                {
                    TransactionId = transaction,
                    Amount = amount,
                    CurrencyCode = currencyCode,
                    TransactionDate = dateTime,
                    Status = status
                });

                transactionCounter++;
            }
            
            if(exceptionList.Count > 0)
            {
                throw new ValidationAggregationException("Transaction creation error", exceptionList);
            }

            return transactionsList;
        }
    }
}
