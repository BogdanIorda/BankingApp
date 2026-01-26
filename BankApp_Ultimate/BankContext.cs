using Microsoft.EntityFrameworkCore;
using MyFirstProject; // This allows us to see the BankAccount class

namespace BankApp_Ultimate
{
    // This class is the "Manager" that handles the database connection
    public class BankContext : DbContext
    {
        // This property tells C# that we have a table called "BankAccounts"
        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        // This method configures the connection (The "Phone Number" of the database)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BankingDb;Integrated Security=True"
            );
        }
    }
}