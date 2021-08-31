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
        public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName)
        {
            if (fileBody.Length < 1)
            {
                throw new ValidationException("File was null", nameof(fileBody));
            }
            var fileSplits = fileBody.Split("\n");
            var piece = (int)(0.5f + 100f * 10 / fileSplits.Length);
            var messageBatchList = new List<TransactionBatchEventDto>();
            var fromLine = 0;
            var toLine = 0;
            if (fileBody.Length > 1)
            {
                for (var i = 0; i < fileSplits.Length; i++)
                {
                    if (toLine - fromLine == piece)
                    {
                        messageBatchList.Add(new TransactionBatchEventDto
                        {
                            From = fromLine,
                            To = toLine,
                            FileName = fileName
                        });
                        fromLine = toLine + 1;
                    }
                    toLine++;
                }
            }
            messageBatchList.Add(new TransactionBatchEventDto
            {
                From = fromLine,
                To = toLine,
                FileName = fileName
            });
            return messageBatchList;
        }

        public IEnumerable<TransactionDto> ParseTransaction(string transactionInfo, string fileBody)
        {
            if (!int.TryParse(transactionInfo.Split("-").First(), out var firstLine))
            {
                throw new ValidationException("Unable to parse lines", nameof(transactionInfo));
            }
            if (!int.TryParse(transactionInfo.Split("-")[1], out var secondLine))
            {
                throw new ValidationException("Unable to parse lines", nameof(transactionInfo));
            }
            var transactionsList = new List<TransactionDto>();
            var fileSplits = fileBody.Split("\n");

            if (firstLine > fileSplits.Length)
            {
                throw new ValidationException("There is no such line in file", nameof(firstLine));
            }
            var parserPattern = "\"([^\"]*)\"";
            var options = RegexOptions.Multiline;
            for (int i = firstLine; i < secondLine; i++)
            {
                var splits = Regex.Matches(fileSplits[i], parserPattern, options)
                    .Select(x => x.Value.Replace("\"", "")).ToList();
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

                transactionsList.Add(new TransactionDto
                {
                    TransactionId = transaction,
                    Amount = amount,
                    CurrencyCode = currencyCode,
                    TransactionDate = dateTime,
                    Status = status
                });
            }
            
            return transactionsList;
        }
    }
}
