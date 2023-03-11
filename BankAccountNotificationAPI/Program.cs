// - The overall larger application's main feature set is to connect to third party banks and aggregate a customer's account information.
// - The purpose of the code below is to provide notifications of changes in their account and to also provide budgeting capabilities.
// - Make assumptions during this implementation to simplify the delivery, list them:
// - E.g. Security to bank provider is not in the solution but it is expected that the bank will enforce a certain technology choice and this can be added later
// - E.g. API level security from consumers will be done using x method for x benefits but this has been excluded from the MVP and listed here as an assumption.

// - Take a business requirement and implement it for the API, written in .NET
// - Describe in summary how the mobile application will respond to the new feature.

// - The solution should be a single .NET solution with any external resources faked.
// - Utilise an in-memory or file based datastore to simplify the exercise.
// - Create a fake third party bank in the solution that will randomly trigger account balance change events to exercise the functionality.

using BankAccountNotificationAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading.Channels;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BankAccountDb>(opt => opt.UseInMemoryDatabase("BankAccountList"));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();
//HttpClient client = new HttpClient();
Random _random = new Random();
//var accounts = app.MapGroup("/accounts");
//accounts.MapGet("/bankaccounts", GetAllAcounts);
//accounts.MapPost("/connectAccount", ConnectAccount);

//For internal-testing purposes: making sure connect account works and save data to database
app.MapGet("/bankaccounts", async (BankAccountDb db) =>
    await db.BankAccounts.ToListAsync());

//MAIN API - Connect to 3rd-Party Fake Bank, send notification of connection, and send notification when balance is changed
app.MapPost("/connectAccount", async (BankAccount account, BankAccountDb db) =>
{
    if (account == null)
    {
        throw new ArgumentException("Account information is required.");
    }
    // validate required input fields data
    if (string.IsNullOrEmpty(account.AccountHolderName) ||
    string.IsNullOrEmpty(account.AccountNumber) ||
    string.IsNullOrEmpty(account.RoutingNumber))
    {
        throw new ArgumentException("All fields are required.");
    }

    // validate required fields
    if (account.AccountHolderName.Length > 30)
    {
        throw new ArgumentException("Invalid account holder name.");
    }
    if (account.AccountNumber.Length < 9 || account.AccountNumber.Length > 18)
    {
        throw new ArgumentException("Account number must be between 9 and 18 digits.");
    }
    if (account.RoutingNumber.Length != 9)
    {
        throw new ArgumentException("Routing number must be 9 digits.");
    }
    if (account.BankId != 131 && account.BankId != 133)
    {
        throw new ArgumentException("Invalid bank ID.");
    }

    // connect to third party bank API
    bool connected = ConnectToBankAPI(account).Result;
    if (connected)
    {
        Console.WriteLine($"Bank account connection successful!");
        db.BankAccounts.Add(account);
        await db.SaveChangesAsync();
        //SendNotificationToCustomer(account);
        // MOBILE --> Notify customer of successful connection 
        //await NotificationService.SendNotificationAsync(customer.MobileDeviceToken, "Bank account connected.");
        return Results.Ok(account.Balance);
    }
    else return Results.NotFound();


});

async Task<bool> ConnectToBankAPI(BankAccount account)
{
    // In the below comments is what it would look like to connect to third party bank API and retrieve balance information:
    // Build the request URL with the provided parameters
    //var requestUrl = $"https://fakebankapi.com/connect?accountNumber={account.AccountNumber}&routingNumber={account.RoutingNumber}&bankId={account.BankId}";
    // Make the API call to the fake third party bank
    //var response = await client.PostAsync(requestUrl, null);
    // Check if the response was successful and return true if it was
    //return response.IsSuccessStatusCode;

    // replace with appropriate code for checking if balance has changed
    account.Balance = 15;
    return true;
}
bool SimulateBalanceChange(BankAccount account)
{
    //    // Simulate a balance change by adding or subtracting a random amount
    decimal change = _random.Next(-100, 100) / 100.0m;
    account.Balance += change;
//    // code to send a notification for balance change:
//    // In a real implementation, this would be sent via the customers mobile app
//    Console.WriteLine($"Balance changed for account {account.AccountNumber}:  {account.Balance}");
    return true;
}
app.MapPut("/changeBalance/{id}", async (int id, BankAccountDb db) =>
{
    var acc_id = await db.BankAccounts.FindAsync(id);

    if (acc_id is null) return Results.NotFound();

    decimal change = _random.Next(-100, 100) / 100.0m;
    acc_id.Balance += change;
    // code to send a notification for balance change:
    // In a real implementation, this would be sent via the customers mobile app
    Console.WriteLine($"Balance changed for account {acc_id.AccountNumber}:  {acc_id.Balance}");
    await db.SaveChangesAsync();

    return Results.Ok(acc_id.Balance);
});

//For internal-testing purposes: making sure connect account works and delete data from database
app.MapDelete("/deleteAccount/{id}", async (int id, BankAccountDb db) =>
{
    if (await db.BankAccounts.FindAsync(id) is BankAccount account)
    {
        db.BankAccounts.Remove(account);
        await db.SaveChangesAsync();
        return Results.Ok(account);
    }

    return Results.NotFound();
});

//app.MapPost("/balance-change", async (BankAccount account, BankAccountDb db) =>
//{
//    // Get account from datastore
//    var selected_account = await db.BankAccounts.FindAsync(account.AccountNumber);

//    // If account doesn't exist, return not found
//    if (account.AccountNumber == null)
//    {
//        return Results.NotFound();
//    }

//    // Generate a random balance change amount
//    var random = new Random();
//    var changeAmount = (decimal)random.NextDouble() * 100;

//    // Determine whether balance should increase or decrease
//    if (random.Next(2) == 0)
//    {
//        changeAmount *= -1;
//    }

//    // Update account balance
//    account.Balance += changeAmount;

//    // Save updated account to datastore
//    //await db.BankAccounts.Update(account.Balance);

//    // Notify connected clients of balance change
//    //await _hub.Clients.Group(account.AccountNumber.ToString()).SendAsync("BalanceChanged", account.Balance);
//    Console.WriteLine($"Balance changed for account {account.AccountNumber}: {account.Balance}");

//    return Results.Created($"/balance-change/{account.Balance}", account);
//});

app.Run();