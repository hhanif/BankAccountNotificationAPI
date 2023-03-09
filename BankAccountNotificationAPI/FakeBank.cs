using System;
using System.Security.Principal;

namespace BankAccountNotificationAPI
{
	public class FakeBank
	{
        public decimal Balance { get; set; } = default!;
        private static readonly Random _random = new Random();

        public void SimulateBalanceChange(BankAccount account)
        {
            // Simulate a balance change by adding or subtracting a random amount
            decimal change = _random.Next(-100, 100) / 100.0m;
            Balance += change;

            Console.WriteLine($"Balance changed for account {account.AccountNumber}: {change:C}");
        }
    }
}

