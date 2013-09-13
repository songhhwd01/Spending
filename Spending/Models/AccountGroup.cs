using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class AccountGroup
	{
		public AccountGroup()
		{
			this.SubGroups = new List<AccountSubGroup>();
		}

		public int Id { get; set; }
		public int UserId { get; set; }
		public string Name { get; set; }
		public byte Order { get; set; }

		public virtual User User { get; set; }
		public virtual ICollection<AccountSubGroup> SubGroups { get; set; }
	}
}