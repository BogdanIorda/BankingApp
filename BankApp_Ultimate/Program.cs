using BankApp_Ultimate;
using MyFirstProject;
using System.Collections.Generic;
using System.Linq;

Console.WriteLine("WELCOME TO THE BANKING SIMULATION");
Console.WriteLine("---------------------------------");

// STRATEGY: KEEP THE LINE OPEN
// We use one "db" block for the whole process.
using (var db = new BankContext())
{
    // 1. LOAD
    Console.WriteLine("Loading accounts from SQL...");
    var myAccounts = db.BankAccounts.ToList();
    Console.WriteLine($"Found {myAccounts.Count} accounts.");

    // 2. MODIFY (Run the simulation)
    Console.WriteLine("\nRunning Transactions...");
    foreach (var account in myAccounts)
    {
        // Because 'db' is still open, it watches this happen!
        // It sees you adding a Transaction to the list.
        try
        {
            account.Withdraw(10); // Charge $10
            Console.WriteLine($" - Charged $10 fee to {account.Owner}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" - Error charging {account.Owner}: {ex.Message}");
        }
    }

    // 3. SAVE
    Console.WriteLine("\nSaving changes...");
    db.SaveChanges(); // Writes the new Balances AND the new Transactions
    Console.WriteLine("Save Complete!");
}

Console.WriteLine("---------------------------------");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();