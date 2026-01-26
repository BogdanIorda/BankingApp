using Microsoft.EntityFrameworkCore;

namespace BankAPI
{
    // This class is the "Manager" that handles the database connection
    public class BankContext : DbContext
    {
        // This property tells C# that we have a table called "BankAccounts"
        public DbSet<BankAccount> BankAccounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public BankContext(DbContextOptions<BankContext> options) : base(options) { }
    }
}