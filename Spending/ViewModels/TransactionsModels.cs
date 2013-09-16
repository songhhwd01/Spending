using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Models
{
	public class TransactionsModel
	{
		public List<Transaction> DownloadedTransactions;
		public List<Transaction> Transactions;
	}

	public class EditSplitModel
	{
		public int Id { get; set; }
		public int CategoryId { get; set; }
		public string Notes { get; set; }
		public decimal Amount { get; set; }
		public bool Delete { get; set; }
	}

	public class EditTransactionModel
	{
		public EditTransactionModel()
		{
			this.Splits = new List<EditSplitModel>();
			this.NewSplits = new List<EditSplitModel>();
		}

		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int AccountId { get; set; }
		public string OriginalDescription { get; set; }
		public string Description { get; set; }
		public bool Credit { get; set; }
		public bool Pending { get; set; }
		public List<EditSplitModel> Splits { get; set; }
		public List<EditSplitModel> NewSplits { get; set; }
	}

	public class CreateSplitModel
	{
		public CreateSplitModel()
		{
			this.NewSplits = new Dictionary<string, EditSplitModel>();
		}

		public Dictionary<string, EditSplitModel> NewSplits { get; set; }
	}
}