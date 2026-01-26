using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BankAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BankController : ControllerBase // this makes the URL: https://localhost:xxxx/Bank
{
    // this is Dependency Injection! the API hands us the database here automatically.
    private readonly BankContext _db;

    public BankController(BankContext db)
    {
        _db = db;
    }

    // this replaces your "View All Accounts" menu option!
    [HttpGet("accounts")]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _db.BankAccounts.ToListAsync();
        return Ok(accounts); // this sends the data as json
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        // 1. find the account in the database using the ID from the web request
        var account = await _db.BankAccounts.FindAsync(request.AccountId);

        // 2. if the account doesn't exist, tell the user
        if (account == null) return NotFound("Account not found!");

        // 3. ppdate the balance
        account.Balance += request.Amount;

        // record the transaction
        _db.Transactions.Add(new Transaction
        {
            BankAccountId = request.AccountId,
            Amount = request.Amount,
            Date = DateTime.Now,
            Note = "Deposit via API"
        });

        // 4. save the changes back to SQL Server
        await _db.SaveChangesAsync();

        // 5. send back the updated account as JSON so the user sees the new balance
        return Ok(account);
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] DepositRequest request) // using deposit cuz it already contains AccountId and Amount
    {
        var account = await _db.BankAccounts.FindAsync(request.AccountId);

        if (account == null) return NotFound("Account not found!");

        // safety Check: prevent overdrafts
        if (account.Balance < request.Amount)
        {
            return BadRequest($"Insufficient funds! You only have {account.Balance.ToString("C")}");
        }

        account.Balance -= request.Amount;

        // record the transaction
        _db.Transactions.Add(new Transaction
        {
            BankAccountId = request.AccountId,
            Amount = -request.Amount, // negative for withdrawal
            Date = DateTime.Now,
            Note = "Withdrawal via API"
        });

        await _db.SaveChangesAsync();

        return Ok(account);
    }

    [HttpGet("history/{accountId}")]
    public async Task<IActionResult> GetHistory(int accountId)
    {
        // fetch transactions belonging to this specific account ID
        var history = await _db.Transactions
            .Where(t => t.BankAccountId == accountId)
            .OrderByDescending(t => t.Date) // newest first
            .ToListAsync();

        return Ok(history);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
    {
        var newAccount = new BankAccount
        {
            Owner = request.Owner,
            Balance = request.InitialBalance
        };

        _db.BankAccounts.Add(newAccount);
        await _db.SaveChangesAsync();

        return Ok(newAccount);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        // 1. find the account
        var account = await _db.BankAccounts.FindAsync(id);

        // 2. if it doesn't exist, tell the user
        if (account == null) return NotFound("Account not found!");

        // 3. remove it from the database table
        _db.BankAccounts.Remove(account);

        // 4. save changes
        await _db.SaveChangesAsync();

        return Ok($"Account {id} deleted successfully!");
    }
}

public class DepositRequest
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
}

public class CreateAccountRequest
{
    public string Owner { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
}