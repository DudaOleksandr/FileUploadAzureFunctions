using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FileUploadMonitor.Core.Parsers
{
    public class JsonParser : IFileParser
    {
        private List<ValidationException> _exceptionList;
        public IEnumerable<TransactionDto> ParseFile(IFormFile file)
        {
            var transactionsList = new List<TransactionDto>();
            _exceptionList = new List<ValidationException>();
            var jsonTransaction = new StringBuilder((int)file.Length);
            using var reader = new StreamReader(file.OpenReadStream());
            var options = RegexOptions.Multiline;
            while (!reader.EndOfStream)
            {
                jsonTransaction.Append(reader.ReadLine());
            }
            var parserPattern = "\\{[^}]*\\}";
            var splits = Regex.Matches(jsonTransaction.ToString(), parserPattern, options).Select(x => x.Value).ToList();

            foreach (var split in splits)
            {
                var transaction = JsonConvert.DeserializeObject<TransactionDto>(
                    split,
                    new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    }
                );
                transactionsList.Add(transaction);
            }
            if (_exceptionList.Count > 0)
            {
                throw new ValidationAggregationException("Transaction creation error", _exceptionList);
            }

            return transactionsList;
        }
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            var source = Regex.Matches(currentError, "'([^']*)'").Select(x => x.Value);
            _exceptionList.Add(new ValidationException("Invalid value", source.FirstOrDefault()?.Replace("\'", "")));
        }
    }
}
