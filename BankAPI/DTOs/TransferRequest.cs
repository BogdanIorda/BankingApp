namespace BankAPI.DTOs
{
    public class TransferRequest
    {
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}