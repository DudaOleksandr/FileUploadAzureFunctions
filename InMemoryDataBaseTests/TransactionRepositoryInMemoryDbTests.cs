using System;
using System.Collections.Generic;
using Common.Enums;
using Common.Exceptions;
using FileUploadMonitor.Domain.Entities;
using FileUploadMonitor.Infrastructure;
using FileUploadMonitor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InMemoryDataBaseTests
{
    public class FakeDbContext : ApplicationDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseInMemoryDatabase("TestDb");
        }
    }
    public class TestsBase : IDisposable
    {
        private readonly FakeDbContext _context;

        public readonly TransactionRepository TransactionRepository;
        
        public TestsBase()
        {
            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptions =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseLazyLoadingProxies()
                    .UseInMemoryDatabase("TestDb");
            _context = new FakeDbContext();
            _context.Database.EnsureCreated();
            TransactionRepository = new TransactionRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
    
    public class TransactionRepositoryInMemoryDbTests
    {
       
        [Fact]
        public void TransactionRepository_AddRange_ShouldHaveExactListSize()
        {
            var testDb = new TestsBase();
            var transactionList = new List<Transaction>()
            {
                new()
                {
                    Id = "Invoice00452849",
                    Amount = 134.3m,
                    CurrencyCode = "USD",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00452893",
                    Amount = 134.3m,
                    CurrencyCode = "RUB",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00452894",
                    Amount = 134.3m,
                    CurrencyCode = "UAH",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                }
            };
             
            testDb.TransactionRepository.AddRange(transactionList);
            List<Transaction> resultList = testDb.TransactionRepository.GetAll(null,null,null,null).Result;
            
            Assert.Equal(transactionList.Count, resultList.Count);
            testDb.Dispose();
        }
        
        [Fact]
        public void TransactionRepository_Add_ShouldAddTransaction()
        {
            var testDb = new TestsBase();
            var expectedTransaction = new Transaction()
            {
                Id = "Invoice00452849",
                Amount = 134.3m,
                CurrencyCode = "USD",
                TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                Status = StatusType.Approved.ToString()
            };
             
            testDb.TransactionRepository.Add(expectedTransaction);
            Transaction resultTransaction = testDb.TransactionRepository.GetById("Invoice00452849");
            
            Assert.Equal(expectedTransaction.Id, resultTransaction.Id);
            testDb.Dispose();
        }
        
        [Fact]
        public void TransactionRepository_Delete_ShouldDeleteTransaction()
        {
            var testDb = new TestsBase();
            var transactionList = new List<Transaction>()
            {
                new()
                {
                    Id = "Invoice00452849",
                    Amount = 134.3m,
                    CurrencyCode = "USD",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00452893",
                    Amount = 134.3m,
                    CurrencyCode = "RUB",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                },
                new()
                {
                    Id = "Invoice00452894",
                    Amount = 134.3m,
                    CurrencyCode = "UAH",
                    TransactionDate = new DateTime(2019, 2, 20, 00, 33, 19),
                    Status = StatusType.Approved.ToString()
                }
            };
             
            testDb.TransactionRepository.AddRange(transactionList);
            
            testDb.TransactionRepository.Delete(transactionList[0]);
            
            List<Transaction> resultList = testDb.TransactionRepository.GetAll(null,null,null,null).Result;
            
            Assert.Equal(transactionList.Count - 1, resultList.Count);
            testDb.Dispose();
        }
    }
}
