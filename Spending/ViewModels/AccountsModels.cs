using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spending.Models;

namespace Spending.ViewModels
{
	public enum ImportState
	{
		Manual,
		Added,
		Removed,
		Matched,
		Reconciled
	}

	public class AccountsModel
	{
		public AccountsModel()
		{
			this.Accounts = new List<AccountModel>();
		}

		public List<AccountModel> Accounts { get; set; }
		public decimal Ending { get; set; }
	}

	public class AccountModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public BoaLogin BoaLogin { get; set; }
		public decimal Ending { get; set; }
	}
}