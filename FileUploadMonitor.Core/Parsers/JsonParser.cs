using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Interfaces;
using Newtonsoft.Json;

namespace FileUploadMonitor.Core.Parsers
{
    public class JsonParser : IFileParser
    {
        public IEnumerable<string> ParseFile(string fileBody, string fileName)
        {
            var jsonTransaction = new StringBuilder(fileBody);
            var options = RegexOptions.Multiline;
            var parserPattern = "\\{[^}]*\\}";
            var splits = Regex.Matches(jsonTransaction.ToString(), parserPattern, options).Select(x => x.Value).ToList();

            //TODO Add logic to get lines of transaction from body
            
            return splits;
        }

        public TransactionDto ParseTransaction(string transactionBody)
        {
            var transaction = JsonConvert.DeserializeObject<TransactionDto>(
                transactionBody,
                new JsonSerializerSettings
                {
                    Error = HandleDeserializationError
                }
            );

            return transaction;
        }

        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            var source = Regex.Matches(currentError, "'([^']*)'").Select(x => x.Value);
            throw new ValidationException("Invalid value", source.FirstOrDefault()?.Replace("\'", ""));
        }
    }
}
