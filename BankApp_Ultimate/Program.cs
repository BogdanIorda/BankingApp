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
    Console.WriteLine("3. Find Custome by Name");
    Console.WriteLine("4. Bank Admin Report");
    Console.WriteLine("5. Exit");
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
        FindCustomer();
    }
    else if (input == "4")
    {
        BankReport();
    }
    else if (input == "5")
    {
        keepRunning = false;
    }
    else
    {
        Console.WriteLine("Invalid option. Press Enter...");
        Console.ReadLine();
    }
}

void WaitForUser()
{
    Console.WriteLine("\nPress Enter to return to menu...");
    Console.ReadLine();
}

// helper methods (this keeps the main code clean)

void ViewHistory()
{
    Console.Clear();
    Console.WriteLine("Fetching data from SQL Database...\n");

    using (var db = new BankContext())
    {
        // using .Include to get the history
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
    WaitForUser();
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
                // adding the transaction
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
    WaitForUser();
}

void FindCustomer()
{
    Console.Clear();
    Console.WriteLine("--- SEARCH FOR CUSTOMER ---");
    Console.Write("Enter the name (or part of it): ");

    string searchName = Console.ReadLine();

    using (var db = new BankContext())
    {
        // LINQ
        // asking the database to filter the list before giving it to us.
        var foundAccount = db.BankAccounts
                             .Include(a => a.Transactions) // Don't forget
                             .Where(a => a.Owner.Contains(searchName)) // The Filter
                             .FirstOrDefault(); // "Give me the first match or null"

        if (foundAccount != null)
        {
            Console.WriteLine($"\nFOUND: {foundAccount.Owner}");
            Console.WriteLine($"Balance: {foundAccount.Balance:C}");
            Console.WriteLine($"Transactions: {foundAccount.Transactions.Count}");
        }
        else
        {
            Console.WriteLine($"\nNo customer found matching '{searchName}'.");
        }
    }

    WaitForUser();
}

void BankReport()
{
    Console.Clear();
    Console.WriteLine("--- BANK ADMIN REPORT ---");
    Console.WriteLine("Gathering statistics from SQL...\n");

    using (var db = new BankContext())
    {
        // 1. TOTAL MONEY (Sum)
        // LINQ asks the DB to add up the 'Balance' column for everyone.
        decimal totalVault = db.BankAccounts.Sum(a => a.Balance);
        Console.WriteLine($"Total Money in Vault: {totalVault:C}"); // what is this :C

        // 2. TOTAL ACCOUNTS (Count)
        int totalAccounts = db.BankAccounts.Count();
        Console.WriteLine($"Total Customers: {totalAccounts}");

        // 3. THE "RICH LIST" (Ordering & Taking)
        // We order by Balance (High to Low) and 'Take' only the top 3.
        var richestCustomers = db.BankAccounts.OrderByDescending(a => a.Balance).Take(3).ToList();

        Console.WriteLine("\n TOP 3 RICHEST CUSTOMERS:");
        foreach (var acc in richestCustomers)
        {
            Console.WriteLine($" - {acc.Owner}: {acc.Balance:C}"); // what is this :C
        }

        // 4. TRANSACTION ACTIVITY (Direct Table Access)
        int totalTransactions = db.Transactions.Count();
        Console.WriteLine($"\n Total Transactions processed: {totalTransactions}");
    }
    WaitForUser();
}