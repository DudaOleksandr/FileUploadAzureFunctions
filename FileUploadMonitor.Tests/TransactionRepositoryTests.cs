using System;
using System.Collections.Generic;
using Common.Enums;
using FileUploadMonitor.Domain.Entities;
using FileUploadMonitor.Infrastructure;
using FileUploadMonitor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;

namespace FileUploadMonitor.Tests
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public void TransactionRepository_AddRange_ShouldHaveExactListSize()
        {

            var transactionList = new List<Transaction>()
            {
                new()
                {
                    Id = "Invoice00052849",
                    Amount = 134.3m,
                    CurrencyCode = "USD",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00052893",
                    Amount = 134.3m,
                    CurrencyCode = "RUB",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00052894",
                    Amount = 134.3m,
                    CurrencyCode = "UAH",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                }
            };
            
            var mockSet = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Transactions).Returns(mockSet.Object);

            var service = new TransactionRepository(mockContext.Object);
            
            service.AddRange(transactionList);
           
            mockSet.Verify(m => m.AddRangeAsync(
                    It.Is<List<Transaction>>(transactions => transactions.Count == transactionList.Count),default),
                $"Expected List lenght is {transactionList.Count} but was other");
            mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
        
        [Fact]
        public void TransactionRepository_Add_ShouldSaveAndCall()
        {

            var transaction = new Transaction
                {
                    Id = "Invoice00052849",
                    Amount = 134.3m,
                    CurrencyCode = "USD",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                };

            var mockSet = new Mock<DbSet<Transaction>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(m => m.Transactions).Returns(mockSet.Object);

            var service = new TransactionRepository(mockContext.Object);
            
            service.Add(transaction);
           
            mockSet.Verify(m => m.Add(It.IsAny<Transaction>()), Times.Once());
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
