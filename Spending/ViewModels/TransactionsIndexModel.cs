using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spending.Models;

namespace Spending.ViewModels
{
	public class TransactionsIndexModel
	{
		public List<Transaction> Transactions { get; set; }
		public Account Account { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int TransactionsPerPage { get; set; }
		public int Page { get; set; }
		public int LastPage { get; set; }
	}
}