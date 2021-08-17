using System;
using System.Net;
using FileUploadMonitor.Domain.Entities;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileUploadMonitor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionStringSql;

        private readonly string _connectionStringCosmos;

        private readonly string _databaseName;


        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext()
        {
            _connectionStringCosmos = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            _connectionStringSql =
                "Data Source=tcp:localhost,1433;Database=TransactionDB; Integrated Security=False;User ID=SA;Password=Prosto1234;";
            _databaseName = "TransactionDb";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                //.UseSqlServer(_connectionStringSql)
                .UseCosmos(_connectionStringCosmos, _databaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .ToContainer("Transactions").HasPartitionKey("CurrencyCode");
        }
    }
}
