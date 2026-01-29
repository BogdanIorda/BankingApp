namespace BankAPI.DTOs
{
    // this is the "Public Mask".
    // it has the same fields as the database, but we CONTROL what goes in here.
    public class AccountDto
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public decimal Balance { get; set; }
    }
}