using System;
using System.Collections.Generic;

namespace MyFirstProject
{
    internal abstract class BankAccount
    {
        protected List<Transaction> allTransactions = new List<Transaction>();
        private static int accountNumberSeed = 1234567890;

        public string Owner { get; set; }
        public string AccountNumber { get; private set; }
        public decimal Balance { get; protected set; }

        public BankAccount(string name, decimal initialBalance)

        {
            this.Owner = name;
            this.AccountNumber = accountNumberSeed.ToString();
            accountNumberSeed++;

            Deposit(initialBalance);
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be positive.");
            }
            else
            {
                Balance += amount;
                var newTransaction = new Transaction(amount, DateTime.Now, "Deposit");
                allTransactions.Add(newTransaction);
            }
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("You cannot withdraw 0 or negative amounts.");
                return;
            }

            if (Balance - amount < 0)
            {
                Console.WriteLine("You don't have enough money");
                return;
            }

            if (amount > 500)
            {
                Console.WriteLine("The maximum amount is 500.");
                return;
            }
            else
            {
                Balance -= amount;
                var newTransaction = new Transaction(-amount, DateTime.Now, "Withdraw");
                allTransactions.Add(newTransaction);
            }
        }

        public string GetAccountHistory()
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("Date\t\tAmount\tNone");

            foreach (var item in allTransactions)
            {
                report.AppendLine($"{item.Date.ToShortDateString()}\t{item.Amount}\t{item.Note}");
            }

            return report.ToString();
        }

        public abstract void PerformMonthEndTransactions();
    }
}