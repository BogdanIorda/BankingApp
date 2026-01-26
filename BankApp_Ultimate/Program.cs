using BankApp_Ultimate;
using Microsoft.EntityFrameworkCore;
using MyFirstProject;
using System;
using System.Linq;

Console.Title = "My First Bank App";
bool keepRunning = true;

while (keepRunning)
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("   BANK OF C# - MAIN MENU");
    Console.WriteLine("=================================");
    Console.WriteLine("1. View All Accounts & History");
    Console.WriteLine("2. Simulate New Transactions ($10 Fee)");
    Console.WriteLine("3. Exit");
    Console.WriteLine("=================================");
    Console.Write("Select an option: ");

    var input = Console.ReadLine();

    if (input == "1")
    {
        ViewHistory();
    }
    else if (input == "2")
    {
        RunSimulation();
    }
    else if (input == "3")
    {
        keepRunning = false;
    }
    else
    {
        Console.WriteLine("Invalid option. Press Enter...");
        Console.ReadLine();
    }
}

// ---------------------------------------------------------
// HELPER METHODS (This keeps the main code clean!)
// ---------------------------------------------------------

void ViewHistory()
{
    Console.Clear();
    Console.WriteLine("Fetching data from SQL Database...\n");

    using (var db = new BankContext())
    {
        // We use .Include to get the history
        var accounts = db.BankAccounts
                         .Include(a => a.Transactions)
                         .ToList();

        foreach (var acc in accounts)
        {
            Console.WriteLine($"ACCOUNT: {acc.Owner} (ID: {acc.Id})");
            Console.WriteLine($"BALANCE: {acc.Balance:C}");
            Console.WriteLine("HISTORY:");
            Console.WriteLine(acc.GetAccountHistory());
            Console.WriteLine("---------------------------------");
        }
    }
    Console.WriteLine("\nPress Enter to return to menu...");
    Console.ReadLine();
}

void RunSimulation()
{
    Console.Clear();
    Console.WriteLine("Running Monthly Fee Simulation...\n");

    using (var db = new BankContext())
    {
        var accounts = db.BankAccounts.ToList();

        foreach (var acc in accounts)
        {
            try
            {
                // We add the transaction
                acc.Withdraw(10);
                Console.WriteLine($" - Charged $10 to {acc.Owner}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" - FAILED: {acc.Owner} ({ex.Message})");
            }
        }

        Console.WriteLine("\nSaving to Database...");
        db.SaveChanges();
        Console.WriteLine("Save Complete!");
    }
    Console.WriteLine("\nPress Enter to return to menu...");
    Console.ReadLine();
}