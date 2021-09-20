using System;
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
        public IEnumerable<TransactionBatchEventDto> ParseFile(string fileBody, string fileName)
        {
            var options = RegexOptions.Multiline;
            var parserPattern = "\\{[^}]*\\}";
            var splits = Regex.Matches(fileBody, parserPattern, options).Select(x => x.Value).ToList();

            //TODO Add logic to get lines of transaction from body
            throw new NotImplementedException("Json type is not supported yet");
            //return splits;
        }

        public IEnumerable<TransactionDto> ParseTransaction(int from, int to, string fileBody)
        {
            var transaction = JsonConvert.DeserializeObject<TransactionDto>(
                fileBody,
                new JsonSerializerSettings
                {
                    Error = HandleDeserializationError
                }
            );
            throw new NotImplementedException("Json type is not supported yet");

            // transaction;
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
