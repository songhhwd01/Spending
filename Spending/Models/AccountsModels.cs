using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Models
{
	public enum ImportState
	{
		Manual,
		Added,
		Removed,
		Matched,
		Reconciled
	}

	public class AccountsIndexModel
	{
		public AccountsIndexModel()
		{
			this.Groups = new List<AccountGroupModel>();
		}

		public List<AccountGroupModel> Groups { get; set; }
		public decimal StartingTotal { get; set; }
		public decimal EndingTotal { get; set; }
	}

	public class AccountGroupModel
	{
		public AccountGroupModel()
		{
			this.SubGroups = new List<AccountSubGroupModel>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public decimal StartingTotal { get; set; }
		public decimal EndingTotal { get; set; }
		public List<AccountSubGroupModel> SubGroups { get; set; }
	}

	public class AccountSubGroupModel
	{
		public AccountSubGroupModel()
		{
			this.Items = new List<AccountModel>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public decimal StartingTotal { get; set; }
		public decimal EndingTotal { get; set; }
		public List<AccountModel> Items { get; set; }
	}

	public class AccountModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Starting { get; set; }
		public decimal Ending { get; set; }
	}
}