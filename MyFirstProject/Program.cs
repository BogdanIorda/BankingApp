using System;
using System.Collections.Generic;

namespace MyFirstProject // Check if your namespace is 'Learning' or 'MyFirstProject'
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("WELCOME TO THE BANKING SIMULATION");
            Console.WriteLine("----------------------------------------");

            // 1. LOAD DATA (The "Thaw" Step)
            // We try to load previous accounts from the hard drive.
            List<BankAccount> accounts = DataService.LoadAccounts();

            // 2. CHECK IF NEW
            // If the list is empty (first time running), we add default accounts.
            if (accounts.Count == 0)
            {
                Console.WriteLine("No saved data found. Creating new accounts...");
                accounts.Add(new InterestEarningAccount("Savings-Standard", 1000));
                accounts.Add(new GiftCardAccount("GiftCard-Birthday", 100));
                accounts.Add(new LineOfCreditAccount("Credit-Emergency", 0));
            }
            else
            {
                Console.WriteLine("Loaded saved data! Welcome back.");
            }

            // 3. SHOW CURRENT STATE
            Console.WriteLine("\nCURRENT BALANCES:");
            foreach (var acc in accounts)
            {
                Console.WriteLine($"   {acc.Owner}: {acc.Balance}");
            }

            // 4. SIMULATE ACTIVITY
            Console.WriteLine("\nRunning Transactions...");
            foreach (var account in accounts)
            {
                if (account is LineOfCreditAccount)
                    account.Withdraw(10);
                else if (account is InterestEarningAccount)
                    account.Deposit(50);
                else
                    account.Withdraw(5);
            }

            // 5. SAVE DATA (The "Freeze" Step)
            // Before we quit, we save the new numbers to the file.
            DataService.SaveAccounts(accounts);
            Console.WriteLine("\nData has been saved to 'bank_data.json'");

            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}