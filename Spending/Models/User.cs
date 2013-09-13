using System;
using System.Collections.Generic;
using System.Linq;

namespace Spending.Models
{
	public class User
	{
		public User()
		{
			this.Settings = new List<Setting>();
			this.AccountGroups = new List<AccountGroup>();
			this.CategoryGroups = new List<CategoryGroup>();
		}

		public int Id { get; set; }
		public string Name { get; set; }

		public IEnumerable<Account> Accounts
		{
			get { return this.AccountGroups.SelectMany(x => x.SubGroups).SelectMany(x => x.Accounts); }
		}

		public IEnumerable<Category> Categories
		{
			get { return this.CategoryGroups.SelectMany(x => x.Categories); }
		}

		public virtual ICollection<Setting> Settings { get; set; }
		public virtual ICollection<AccountGroup> AccountGroups { get; set; }
		public virtual ICollection<CategoryGroup> CategoryGroups { get; set; }
	}
}