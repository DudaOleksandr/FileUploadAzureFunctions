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
        public IEnumerable<string> ParseFile(string fileBody, string fileName)
        {
            if (fileBody.Length > 1)
            {
                var fileSplits = fileBody.Split("\n");
                for (var i = 1; i < fileSplits.Length + 1; i++)
                {
                    fileSplits[i - 1] = $"{i} - {i}, {fileName}";
                }
                return fileSplits;
            }
            throw new ValidationException("File was null", nameof(fileBody));
        }

        public TransactionDto ParseTransaction(string transactionInfo, string fileBody)
        {
            if (!int.TryParse(transactionInfo.Split("-").First(), out var firstLine) &&
                !int.TryParse(transactionInfo.Split("-").First(), out var secondLine))
            {
                throw new ValidationException("Unable to parse lines", nameof(transactionInfo));
            }

            var fileSplits = fileBody.Split("\n");

            if (firstLine > fileSplits.Length)
            {
                throw new ValidationException("There is no such line in file", nameof(firstLine));
            }
            var parserPattern = "\"([^\"]*)\"";
            var options = RegexOptions.Multiline;
            var splits = Regex.Matches(fileSplits[firstLine - 1], parserPattern, options).Select(x => x.Value.Replace("\"", "")).ToList();

            if (splits.Count != 5)
            {
                throw new ValidationException("There is invalid file structure", nameof(splits));
            }
            var transaction = splits[0];

            if (!decimal.TryParse(splits[1], out var amount))
            {
                throw new ValidationException("Invalid value", $"transaction.{transaction}.{nameof(amount)}");
            }
            var currencyCode = splits[2];

            if (!DateTime.TryParseExact(splits[3], "dd/MM/yyyy hh:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dateTime))
            {
                throw new ValidationException("Invalid value", $"transaction.{transaction}.{nameof(dateTime)}");
            }
            if (!Enum.TryParse(splits[4], true, out StatusType status))
            {
                throw new ValidationException("Invalid value", $"transaction.{transaction}.{nameof(status)}");
            }
            return new TransactionDto
            {
                TransactionId = transaction,
                Amount = amount,
                CurrencyCode = currencyCode,
                TransactionDate = dateTime,
                Status = status
            };
        }
    }
}
