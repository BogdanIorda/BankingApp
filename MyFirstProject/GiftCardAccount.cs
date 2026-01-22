namespace MyFirstProject
{
    internal class GiftCardAccount : BankAccount

    {
        public GiftCardAccount(string name, decimal initialBalance) : base(name, initialBalance)
        {
        }

        public override void PerformMonthEndTransactions()
        {
            if (Balance > 0)
            {
                Withdraw(Balance);
            }
        }
    }
}