using FileUploadMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FileUploadMonitor.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Transaction> Transactions { get; set; }

        public ApplicationDbContext()
        {
            _connectionString =
                "Data Source=tcp:localhost,1433;Database=TransactionDB; Integrated Security=False;User ID=SA;Password=Prosto1234;";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(_connectionString);
        }

    }
}
