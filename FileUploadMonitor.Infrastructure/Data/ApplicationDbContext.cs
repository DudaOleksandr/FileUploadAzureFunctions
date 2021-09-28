using System;
using FileUploadMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileUploadMonitor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionStringSql = Environment.GetEnvironmentVariable("ConnectionStringSql");

        private readonly string _connectionStringCosmos = Environment.GetEnvironmentVariable("ConnectionStringCosmos");

        private readonly string _databaseName = Environment.GetEnvironmentVariable("DatabaseName");

        private readonly bool _useCosmosDb = bool.TryParse(Environment.GetEnvironmentVariable("UseCosmosDb"), out bool value) && value;
        
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_useCosmosDb)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseCosmos(_connectionStringCosmos, _databaseName);
            }
            else
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer(_connectionStringSql);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>()
                .ToContainer("Transactions").HasPartitionKey("CurrencyCode");
        }
    }
}
