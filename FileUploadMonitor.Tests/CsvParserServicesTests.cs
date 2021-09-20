using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using Common.Exceptions;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Core.Parsers;
using Xunit;

namespace FileUploadMonitor.Tests
{
    public class CsvParserServicesTests
    {
        [Fact]
        public void Csv_ParseTransaction_ShouldReturnListOfTransactionDtoItems()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"";
            var from = 0;
            var to = 3;
            var transactionsList = new List<TransactionDto>
            {
                new()
                {
                    TransactionId = "Invoice00050849",
                    Amount = 52513.4m,
                    CurrencyCode = "USD",
                    Status = StatusType.Failed,
                    TransactionDate = new DateTime(2019,2,20,00,33,19)
                },
                new()
                {
                    TransactionId = "Invoice00050893",
                    Amount = 158323.4m,
                    CurrencyCode = "USD",
                    Status = StatusType.Approved,
                    TransactionDate = new DateTime(2019,2,21,2,4,59)
                },
                new()
                {
                    TransactionId = "Invoice00050894",
                    Amount = 45575.9m,
                    CurrencyCode = "RUB",
                    Status = StatusType.Failed,
                    TransactionDate = new DateTime(2019,02,20,00,33,20)
                }
            };
            
            var parser = new CsvParser();

            var res = parser.ParseTransaction(from, to, fileBody).ToList();
            foreach ((TransactionDto x, TransactionDto y) transactionsTuple in transactionsList.Zip(res, (x, y) => (x, y)))
            {
                Assert.Equal(transactionsTuple.x, transactionsTuple.y);
            }
        }

        [Fact]
        public void Csv_ParseTransaction_ShouldReturnValidationExceptionNoSuchLine()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"USD\", \"Failed\"\n";
            var from = 14;
            var to = 0;
            var parser = new CsvParser();
            var expectedException = new ValidationException("There is no such line in file", "from");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseTransaction(from, to, fileBody));
            Assert.Equal(expectedException.Message, actualException.Message);
        }
        
        [Fact]
        public void Csv_ParseTransaction_ShouldReturnValidationExceptionInvalidStructure()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"Failed\"\n";
            var from = 0;
            var to = 1;
            var parser = new CsvParser();
            var expectedException = new ValidationException("There is invalid file structure", "from");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseTransaction(from, to, fileBody));
            Assert.Equal(expectedException.Message, actualException.Message);
        }
        
        [Fact]
        public void Csv_ParseTransaction_ShouldReturnValidationExceptionInvalidValueAmount()
        {
            var fileBody = "\"Invoice00050849\", \"5d25s13.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n";
            var from = 0;
            var to = 1;
            var parser = new CsvParser();
            var expectedException = new ValidationException("Invalid value", "transaction.Invoice00050849.amount");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseTransaction(from, to, fileBody));
            Assert.Equal(expectedException.Source, actualException.Source);
        }
        
        [Fact]
        public void Csv_ParseTransaction_ShouldReturnValidationExceptionInvalidValueDateTime()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/0fsd2/2019 12:3sfd3:19\", \"Failed\"\n";
            var from = 0;
            var to = 1;
            var parser = new CsvParser();
            var expectedException = new ValidationException("Invalid value", "transaction.Invoice00050849.dateTime");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseTransaction(from, to, fileBody));
            Assert.Equal(expectedException.Source, actualException.Source);
        }
        
        [Fact]
        public void Csv_ParseTransaction_ShouldReturnValidationExceptionInvalidValueStatus()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"FF\"\n";
            var from = 0;
            var to = 1;
            var parser = new CsvParser();
            var expectedException = new ValidationException("Invalid value", "transaction.Invoice00050849.status");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseTransaction(from, to, fileBody));
            Assert.Equal(expectedException.Source, actualException.Source);
        }
        
        [Fact]
        public void Csv_ParseFile_ShouldReturnListOfTransactionBatchEventDtoItems()
        {
            var fileBody = "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" +
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"\n" +
                           "\"Invoice00050894\", \"45575.9\", \"RUB\", \"20/02/2019 12:33:20\", \"Failed\"\n" + 
                           "\"Invoice00050849\", \"52513.4\", \"USD\", \"20/02/2019 12:33:19\", \"Failed\"\n" +
                           "\"Invoice00050893\", \"158323.4\", \"USD\", \"21/02/2019 02:04:59\", \"Approved\"";
            
            var fileName = "csv/myFile0.csv";
            var transactionsList = new List<TransactionBatchEventDto>
            {
                new()
                {
                    From = 0,
                    To = 26,
                    FileName = fileName
                },
                new()
                {
                    From = 27,
                    To = 38,
                    FileName = fileName
                }
            };
            
            var parser = new CsvParser();

            IEnumerable<TransactionBatchEventDto> res = parser.ParseFile(fileBody, fileName);
            foreach ((TransactionBatchEventDto x, TransactionBatchEventDto y) transactionsTuple in transactionsList.Zip(res, (x, y) => (x, y)))
            {
                Assert.Equal(transactionsTuple.x, transactionsTuple.y);
            }
        }
        
         [Fact]
        public void Csv_ParseFile_ShouldReturnValidationExceptionFileNull()
        {
            var fileBody = string.Empty;
            
            var fileName = "csv/myFile0.csv";
            
            var parser = new CsvParser();

            var expectedException = new ValidationException("File was null", "fileBody");
           
            ValidationException actualException =
                Assert.Throws<ValidationException>(() => parser.ParseFile(fileBody, fileName));
            Assert.Equal(expectedException.Message, actualException.Message);
        }
    }
}
