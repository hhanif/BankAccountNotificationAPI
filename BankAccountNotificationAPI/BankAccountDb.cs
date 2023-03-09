using System;
using Microsoft.EntityFrameworkCore;

namespace BankAccountNotificationAPI
{
	public class BankAccountDb : DbContext
    {
		public BankAccountDb(DbContextOptions<BankAccountDb> options) : base(options) { }
        public DbSet<BankAccount> BankAccounts => Set<BankAccount>();

    }
}

