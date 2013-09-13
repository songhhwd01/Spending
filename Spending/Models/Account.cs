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
		public string Name { get; set; }
		public decimal Balance { get; set; }
		public int SubGroupId { get; set; }
		public bool Owned { get; set; }
		public int Order { get; set; }

		public virtual AccountSubGroup SubGroup { get; set; }
		public virtual ICollection<Transaction> Transactions { get; set; }
	}
}