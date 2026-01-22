using System;

namespace MyFirstProject
{
    public class LineOfCreditAccount : BankAccount

    {
        public LineOfCreditAccount(string name, decimal initialBalance) : base(name, initialBalance)
        {
        }

        public override void PerformMonthEndTransactions()
        {
            if (Balance < 0)
            {
                decimal interest = -Balance * 0.07m;
                Withdraw(interest);
            }
        }

        public override void Withdraw(decimal amount)
        {
            Balance -= amount;
            var withdrawal = new Transaction(-amount, DateTime.Now, "Withdrawal");
            allTransactions.Add(withdrawal);
        }
    }
}