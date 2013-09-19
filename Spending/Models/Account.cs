using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Spending.Models
{
	public class Account
	{
		public Account()
		{
			this.Transactions = new List<Transaction>();
		}

		public int Id { get; set; }
		public int UserId { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public string BoaAccountRefNum { get; set; }

		public virtual User User { get; set; }
		public virtual ICollection<Transaction> Transactions { get; set; }
	}
}