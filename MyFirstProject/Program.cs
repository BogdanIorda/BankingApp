using System;
using System.Collections.Generic;

namespace MyFirstProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("🏦 WELCOME TO THE BANKING SIMULATION 🏦");
            Console.WriteLine("----------------------------------------");

            // 1. Create the accounts (Polymorphism: Different types in one list!)
            var accounts = new List<BankAccount>
            {
                new InterestEarningAccount("Savings-Standard", 1000),
                new GiftCardAccount("GiftCard-Birthday", 100),
                new LineOfCreditAccount("Credit-Emergency", 0)
            };

            // 2. SIMULATE ACTIVITY (Spending and Saving)
            Console.WriteLine("running transactions...");
            foreach (var account in accounts)
            {
                // Checking the type to do specific things!
                if (account is LineOfCreditAccount)
                {
                    // Force the credit line into debt to test the fee
                    account.Withdraw(50);
                }
                else if (account is InterestEarningAccount)
                {
                    // Add more savings to test interest calculation
                    account.Deposit(500);
                }
                else // It must be a Gift Card
                {
                    // Spend some of the gift card
                    account.Withdraw(20);
                }

                // Show the balance after the transaction
                Console.WriteLine($"[Account: {account.Owner}] Current Balance: {account.Balance}");
            }

            Console.WriteLine("\n----------------------------------------");
            Console.WriteLine("END OF MONTH PROCESSING...");
            Console.WriteLine("----------------------------------------");

            // 3. TRIGGER MONTH-END
            foreach (var account in accounts)
            {
                // This one line behaves differently for every object!
                account.PerformMonthEndTransactions();

                Console.WriteLine($"   Account: {account.Owner}");
                Console.WriteLine($"   Final Balance: {account.Balance}");
                Console.WriteLine("   ----------------");
                Console.WriteLine();
            }

            // 4. PRINT HISTORY
            foreach (var account in accounts)
            {
                Console.WriteLine(account.GetAccountHistory());
                Console.WriteLine("........................................\n");
            }

            Console.Write("Press any key to close...");
            Console.ReadKey();
        }
    }
}