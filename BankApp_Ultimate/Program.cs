using BankApp_Ultimate; // Needed to see 'BankContext'
using MyFirstProject;
using System.Collections.Generic;
using System.Linq; // Needed for database queries

// ... standard program setup ...

Console.WriteLine("WELCOME TO THE BANKING SIMULATION");
Console.WriteLine("---------------------------------");

// --- 🧪 SQL TEST START ---
// We open a connection to the database
using (var db = new BankContext())
{
    Console.WriteLine("Attempting to connect to SQL Database...");

    // This single line converts SQL rows into C# Objects!
    // It's the equivalent of "SELECT * FROM BankAccounts"
    var sqlAccounts = db.BankAccounts.ToList();

    Console.WriteLine($"Success! Found {sqlAccounts.Count} accounts in the database:");

    foreach (var acc in sqlAccounts)
    {
        Console.WriteLine($" - Owner: {acc.Owner} | Balance: {acc.Balance}");
    }
}
Console.WriteLine("---------------------------------");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
// --- 🧪 SQL TEST END ---