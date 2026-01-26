using BankApp_Ultimate;
using Microsoft.EntityFrameworkCore;
using MyFirstProject;
using System;
using System.Linq;

Console.Title = "My First Bank App";

InitializeDatabase();

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
    Console.WriteLine("5. Transfer Funds");
    Console.WriteLine("6. Delete Account");
    Console.WriteLine("7. Exit");
    Console.WriteLine("=================================");
    Console.Write("Select an option: ");

    var input = Console.ReadLine();

    if (input == "1") ViewHistory();
    else if (input == "2") await RunSimulation();
    else if (input == "3") FindCustomer();
    else if (input == "4") BankReport();
    else if (input == "5") TransferFunds();
    else if (input == "6") DeleteAccount();
    else if (input == "7") keepRunning = false;
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

// this method uses Async/Await to prevent UI freezing during large database updates
async Task RunSimulation()
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

        await db.SaveChangesAsync();
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
        Console.WriteLine($"Total Money in Vault: {totalVault:C}"); // C to convert into money format and sign

        // 2. TOTAL ACCOUNTS (Count)
        int totalAccounts = db.BankAccounts.Count();
        Console.WriteLine($"Total Customers: {totalAccounts}");

        // 3. THE "RICH LIST" (Ordering & Taking)
        // We order by Balance (High to Low) and 'Take' only the top 3.
        var richestCustomers = db.BankAccounts.OrderByDescending(a => a.Balance).Take(3).ToList();

        Console.WriteLine("\n TOP 3 RICHEST CUSTOMERS:");
        foreach (var acc in richestCustomers)
        {
            Console.WriteLine($" - {acc.Owner}: {acc.Balance:C}");
        }

        // 4. TRANSACTION ACTIVITY (Direct Table Access)
        int totalTransactions = db.Transactions.Count();
        Console.WriteLine($"\n Total Transactions processed: {totalTransactions}");
    }
    WaitForUser();
}

void TransferFunds()
{
    Console.Clear();
    Console.WriteLine("--- TRANSFER FUNDS ---");

    using (var db = new BankContext())
    {
        // 1. get the sender
        Console.Write("Enter SENDER Accounter ID: ");
        if (!int.TryParse(Console.ReadLine(), out int fromId)) return;

        var fromAccount = db.BankAccounts.Find(fromId); // Find() is a shortcut for searching by ID!

        if (fromAccount == null)
        {
            Console.WriteLine("Sender Account no found!");
            WaitForUser();
            return;
        }

        // 2. get the receiver
        Console.Write("Enter RECEIVER Account ID: ");
        if (!int.TryParse(Console.ReadLine(), out int toId)) return;

        var toAccount = db.BankAccounts.Find(toId);

        if (toAccount == null)
        {
            Console.WriteLine("Receiver Account not found!");
            WaitForUser();
            return;
        }

        // 3. get the amount
        Console.WriteLine($"\nSending from {fromAccount.Owner} ({fromAccount.Balance}) -> to -> {toAccount.Owner}");
        Console.Write("Enter Amount to Transfer: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount)) return;

        // 4. the logic
        // verifing found first
        if (fromAccount.Balance >= amount)
        {
            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);

            Console.WriteLine("\nSaving transactio...");
            db.SaveChanges(); //writes both changes in one go.
            Console.WriteLine("Transfer Complete!");
        }
        else
        {
            Console.WriteLine("Insufficient funds!");
        }
    }
    WaitForUser();
}

void DeleteAccount()
{
    Console.Clear();
    Console.WriteLine("--- DELETE ACCOUNT ---");
    Console.Write("Enter Account ID to DELETE: ");

    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    using (var db = new BankContext())
    {
        // 1. find the Account and its Luggage (Transactions)
        // must use .Include() here. If we don't load the transactions,
        // can't delete them!

        var accountToDelete = db.BankAccounts.Include(a => a.Transactions).FirstOrDefault(a => a.Id == id);

        if (accountToDelete == null)
        {
            Console.WriteLine("Account no found.");
            WaitForUser();
            return;
        }

        Console.WriteLine($"\nWARNING: You are about to delete {accountToDelete.Owner}");
        Console.WriteLine($"This will delete thier account and {accountToDelete.Transactions.Count} transaction history records.");
        Console.Write("Are you sure? (Type 'YES' to confirm): ");

        if (Console.ReadLine() == "YES")
        {
            // 3. REMOVE THE CHILDREN (Transactions)
            // tell the database: "Mark all these transactions for deletion"
            db.Transactions.RemoveRange(accountToDelete.Transactions);

            // 4. REMOVE THE PARENT (Account)
            db.BankAccounts.Remove(accountToDelete);

            // here actually happens (the deletion)
            db.SaveChanges();
            Console.WriteLine("Account and History Deleted.");
        }
        else
        {
            Console.WriteLine("Cancelled.");
        }
    }
    WaitForUser();
}

void InitializeDatabase()
{
    {
        using (var db = new BankContext())
        {
            var numCustomers = 100;

            // 1. check if we already have customers, STOP.
            if (db.BankAccounts.Any())
            {
                return;
            }

            Console.WriteLine($"Initializing Databse with {numCustomers} new customers...");

            for (int i = 1; i <= 100; i++)
            {
                string name = $"Customer {i}";
                decimal startBalance = 100 + (i * 10);

                var newAccount = new BankAccount(name, startBalance);
                newAccount.Withdraw(5);

                db.BankAccounts.Add(newAccount);
            }

            db.SaveChanges();
            Console.WriteLine($"Initialization Complete! {numCustomers} Accounts Created.");
        }
    }
}