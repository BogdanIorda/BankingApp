using System;

namespace MyFirstProject
{
    public class InterestEarningAccount : BankAccount
    {
        public InterestEarningAccount(string name, decimal initialBalance) : base(name, initialBalance)
        {
        }

        public override void PerformMonthEndTransactions()
        {
            if (Balance > 500)
            {
                decimal interest = Balance * 0.05m;
                Deposit(interest);
                Console.WriteLine($"Check it out! {Owner} earned interest: {interest}");
            }
        }
    }
}