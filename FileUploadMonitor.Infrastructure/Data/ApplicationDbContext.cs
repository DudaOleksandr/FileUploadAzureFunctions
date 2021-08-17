using FileUploadMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileUploadMonitor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionStringSql;

        private readonly string _connectionStringCosmos;

        private readonly string _databaseName;

        public bool UseCosmosDb = false;

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
            if (UseCosmosDb)
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
