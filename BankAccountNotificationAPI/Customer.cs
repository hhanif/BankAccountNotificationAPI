using System;
using System.Security.Principal;

namespace BankAccountNotificationAPI
{
	public class Customer
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BankAccount> Accounts { get; set; }
    }
}

