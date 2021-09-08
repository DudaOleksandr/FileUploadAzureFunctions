using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileUploadMonitor.Core.Dtos;
using FileUploadMonitor.Domain.Entities;
using FileUploadMonitor.Domain.Interfaces;

namespace FileUploadMonitor.Core.Services
{
    class TransactionsService : ITransactionsService
    {
        private readonly ITransactionRepository _transactionRepository;
        
        public TransactionsService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        
        public async Task<IEnumerable<TransactionDto>> Save(List<TransactionDto> transactionModels)
        {
            var transactionList = new List<Transaction>();
            foreach (var transactionModel in transactionModels)
            {
                var transaction = new Transaction
                {
                    Id = transactionModel.TransactionId,
                    TransactionDate = (DateTime)transactionModel.TransactionDate,
                    Status = transactionModel.Status.ToString(),
                    CurrencyCode = transactionModel.CurrencyCode,
                    Amount = (decimal)transactionModel.Amount
                };
                transactionList.Add(transaction);
            }
            await _transactionRepository.AddRange(transactionList);
            return transactionModels;
        }

        public async Task<List<OutputTransactionDto>> Get(string currency, string status, string dateFrom, string dateTo)
        {
            DateTime? parsedDateFrom = null;
            DateTime? parsedDateTo = null;
            if (dateFrom is not null && dateTo is not null)
            {
                if (!DateTime.TryParse(dateFrom, out var date1) && DateTime.TryParse(dateTo, out var date2))
                {
                    parsedDateFrom = date1;
                    parsedDateTo = date2;
                }
            }

            var transactions = await _transactionRepository.GetAll(currency, status, parsedDateFrom, parsedDateTo);
            var outputList = new List<OutputTransactionDto>();
            foreach (var transaction in transactions)
            {
                outputList.Add(
                    new OutputTransactionDto()
                    {
                        Id = transaction.Id,
                        Payment = transaction.Amount + " " + transaction.CurrencyCode,
                        Status = StatusTypeSlicing(transaction.Status)
                    }
                );
            }

            return outputList;
        }
        
        private static string StatusTypeSlicing(string status)
        {
            return status switch
            {
                "Approved" => "A",
                "Failed" or "Rejected" => "R",
                "Finished" or "Done" => "D",
                _ => "",
            };
        }

    }
}
