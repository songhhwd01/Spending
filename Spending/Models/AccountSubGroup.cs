using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class AccountSubGroup
	{
		public AccountSubGroup()
		{
			this.Accounts = new List<Account>();
		}

		public int Id { get; set; }
		public int GroupId { get; set; }
		public string Name { get; set; }
		public byte Order { get; set; }

		public virtual AccountGroup Group { get; set; }
		public virtual ICollection<Account> Accounts { get; set; }
	}
}