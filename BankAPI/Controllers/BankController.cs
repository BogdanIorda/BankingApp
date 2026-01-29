using BankAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    // GET: api/Bank/accounts

    // THE LOCK: You MUST have a valid Token(Badge) to enter here.
    // If you don't send a token, the API replies with "401 Unauthorized".
    [Authorize]
    [HttpGet("accounts")]
    public async Task<ActionResult<List<AccountDto>>> GetAllAccounts()

    {
        // 1. get the raw data from the database
        var accounts = await _db.BankAccounts.ToListAsync();

        // 2. map (convert) them to DTOs
        // act like a factory: Raw Material -> Finished Product
        var safeAccounts = accounts.Select(account => new AccountDto
        {
            Id = account.Id,
            Owner = account.Owner,
            Balance = account.Balance,
        }).ToList();

        return Ok(safeAccounts);
    }

    [Authorize] // <--- 1. Lock (User must be logged in)
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        // 2. READ THE BADGE (Extract User ID from the Token)
        // The "User" object is created automatically by the [Authorize] lock
        var idClaim = User.FindFirst("AccountId");
        if (idClaim == null)
        {
            return Unauthorized("Invalid Token: No Account ID found.");
        }

        // Convert the text ID (from token) to a number
        int myId = int.Parse(idClaim.Value);

        // 3. Find the account using the ID from the token
        var account = await _db.BankAccounts.FindAsync(myId);

        if (account == null)
        {
            return NotFound("Account not found.");
        }
        // 4. Update the balance
        account.Balance += request.Amount;
        await _db.SaveChangesAsync();

        return Ok(new { Message = "Deposit Successful", NewBalance = account.Balance });
    }

    [Authorize] // <--- 1. Locked
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] DepositRequest request)
    {
        // 2. READ THE BADGE (Get ID from Token)
        var idClaim = User.FindFirst("AccountId");
        if (idClaim == null) return Unauthorized("Invalid Token.");

        int myId = int.Parse(idClaim.Value);

        // 3. Find the account
        var account = await _db.BankAccounts.FindAsync(myId);
        if (account == null) return NotFound("Account not found.");

        // 4. Safety Checks
        if (request.Amount <= 0) return BadRequest("Amount must be positive.");
        if (account.Balance < request.Amount) return BadRequest("Insufficient funds!");

        // 4. Update the balance
        account.Balance -= request.Amount;
        await _db.SaveChangesAsync();

        return Ok(new { Message = "Withdrawal Successful", NewBalance = account.Balance });
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        // 1. find the Sender and the Receiver in the database

        var sender = await _db.BankAccounts.FindAsync(request.FromAccountId);
        var receiver = await _db.BankAccounts.FindAsync(request.ToAccountId);

        // 2. safety shecks
        if (sender == null || receiver == null) return NotFound("One of the accounts does not exist.");

        if (request.Amount <= 0) return BadRequest("Amount must be positive.");

        if (sender.Id == receiver.Id) return BadRequest("You cannot transfer money to yourself.");

        if (sender.Balance < request.Amount) return BadRequest("Insufficinet funds.");

        // 3. move the money (In Memory)
        sender.Balance -= request.Amount;
        receiver.Balance += request.Amount;

        // 4. add "Paper Trail" (History) for BOTH sides

        CreateTransaction(sender.Id, -request.Amount, $"Transfer to Account {receiver.Id}");

        CreateTransaction(receiver.Id, request.Amount, $"Transfer from Account {sender.Id}");

        await _db.SaveChangesAsync();

        return Ok($"Success! Transferred {request.Amount:C} from {sender.Owner} to {receiver.Owner}.");
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
            Balance = request.InitialBalance,
        };

        _db.BankAccounts.Add(newAccount);
        await _db.SaveChangesAsync();
        var accountDto = new AccountDto
        {
            Id = newAccount.Id,
            Owner = newAccount.Owner,
            Balance = newAccount.Balance,
        };

        return Ok(accountDto);
    }

    [HttpDelete("delete/{id}")]
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

    // helper method
    private void CreateTransaction(int accountId, decimal amount, string note)
    {
        _db.Transactions.Add(new Transaction
        {
            BankAccountId = accountId,
            Amount = amount,
            Date = DateTime.Now,
            Note = note
        });
    }

    // POST: api/Bank/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        // 1. find the user by Name (Owner)
        // use "FirstOrDefault" because there might be no user with that name.
        var user = await _db.BankAccounts.FirstOrDefaultAsync(u => u.Owner == request.Owner);
        // 2. check User and PIN

        if (user == null || user.Pin != request.Pin)
        {
            return Unauthorized("Invalid Owner or PIN.");
        }

        // 3. create the "Claims" (The info ON the badge)
        // put the User's ID and Name inside the token so we know who they are later.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Owner),
            new Claim("AccountId", user.Id.ToString()) // custom claim for our Bank
        };

        // 4. create the "Key" (The Secret Stamp)
        // IN REAL LIFE: Store this in a secure file (appsettings.json), NOT here!
        // It must be at least 32 characters long.
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyTopSecretBankKeyThatIsVeryLongAndSecure123!"));

        // 5. create the Credentials (Signing the badge)
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 6. generate the Token Object
        var token = new JwtSecurityToken(
            issuer: "BankAPI",
            audience: "BankUsers",
            claims: claims,
            expires: DateTime.Now.AddHours(1), // works for 1 hour
            signingCredentials: creds
        );

        // 7. conevert to string and return
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = jwt });
    }
}

public class TransferRequest
{
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public decimal Amount { get; set; }
}

public class CreateAccountRequest
{
    public string Owner { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
}