using BankApp_Ultimate; // Needed to see 'BankContext'
using System.Collections.Generic;
using System.Linq;

namespace MyFirstProject // Keep your original namespace
{
    public static class DataService
    {
        // OLD: Read from JSON file
        // NEW: Read from SQL Database
        public static List<BankAccount> LoadAccounts()
        {
            using (var db = new BankContext())
            {
                // .ToList() executes the SELECT * query automatically
                return db.BankAccounts.ToList();
            }
        }

        // OLD: Write to JSON file
        // NEW: Update or Insert into SQL Database
        public static void SaveAccounts(List<BankAccount> accounts)
        {
            using (var db = new BankContext())
            {
                // We use .Update() because it's magic:
                // 1. If the Account has ID=0, it knows it's NEW -> It does an INSERT.
                // 2. If the Account has ID>0, it knows it Exists -> It does an UPDATE.
                db.BankAccounts.UpdateRange(accounts);

                // This effectively pushes the "Commit" button on the database
                db.SaveChanges();
            }
        }
    }
}