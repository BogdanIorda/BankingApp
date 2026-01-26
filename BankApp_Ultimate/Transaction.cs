using System;

namespace MyFirstProject
{
    public class Transaction
    {
        // 1. Primary Key (So SQL can find this specific row)
        public int Id { get; set; }

        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        // 2. Foreign Key (The "Luggage Tag" pointing to the Owner)
        // This MUST match the Primary Key type of BankAccount (int)
        public int BankAccountId { get; set; }

        // 3. Empty Constructor (For Entity Framework)
        public Transaction()
        { }

        public Transaction(decimal amount, DateTime date, string note)
        {
            this.Amount = amount;
            this.Date = date;
            this.Note = note;
        }
    }
}