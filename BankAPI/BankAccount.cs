using System;
using System.Collections.Generic;
using System.Text;

namespace BankAPI
{
    public class BankAccount
    {
        // ==========================================
        // 1. FIELDS (Private internal storage)
        // ==========================================
        private static int accountNumberSeed = 1234567890;

        // ==========================================
        // 2. PROPERTIES (Public data for DB/World)
        // ==========================================
        public int Id { get; set; }

        public string Owner { get; set; }
        public string Number { get; }
        public decimal Balance { get; set; }

        // This is a Property because it has { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        // ==========================================
        // 3. CONSTRUCTORS (Setup)
        // ==========================================
        public BankAccount()
        { }

        public BankAccount(string name, decimal initialBalance)
        {
            this.Owner = name;
            this.Balance = initialBalance;
            this.Number = accountNumberSeed.ToString();
            accountNumberSeed++;
            Deposit(initialBalance);
        }

        // ==========================================
        // 4. METHODS (Actions)
        // ==========================================
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be positive.");
                return;
            }

            Balance += amount;

            var newTransaction = new Transaction(amount, DateTime.Now, "Deposit");
            newTransaction.BankAccountId = this.Id;
            Transactions.Add(newTransaction);
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be positive.");
                return;
            }
            if (Balance - amount < 0)
            {
                Console.WriteLine($"Insufficient funds for {Owner}.");
                return;
            }

            Balance -= amount;

            var newTransaction = new Transaction(-amount, DateTime.Now, "Withdraw");
            newTransaction.BankAccountId = this.Id;
            Transactions.Add(newTransaction);
        }

        public string GetAccountHistory()
        {
            var report = new StringBuilder();
            report.AppendLine("Date\t\tAmount\tNote");
            foreach (var item in Transactions)
            {
                report.AppendLine($"{item.Date.ToShortDateString()}\t{item.Amount}\t{item.Note}");
            }
            return report.ToString();
        }

        public virtual void PerformMonthEndTransactions()
        { }
    }
}