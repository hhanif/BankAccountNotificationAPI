using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace BankAccountNotificationAPI
{
	public class BankAccount
	{
        public int Id { get; set; } //This is the existing customer ID, used to track as all the below variables are for the 3rd-party bank
        public string AccountHolderName { get; set; } = default!;
        public string AccountNumber { get; set; } = default!;
        public string RoutingNumber { get; set; } = default!;
        public int BankId { get; set; } = default!;
        public decimal Balance { get; set; } = default!;

    }
}

