using BankAPI; // make sure this matches your namespace!
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. tell the API about your Bank Database
builder.Services.AddDbContext<BankContext>(options =>
    options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BankingDb;Integrated Security=True"));

// 2. tell the app we want to use JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "BankAPI",
        ValidAudience = "BankUsers",
        // MUST match the long key you used in the Controller!
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MyTopSecretBankKeyThatIsVeryLongAndSecure123!"))
    };
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();