using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class Transaction
	{
		public Transaction()
		{
			this.Splits = new List<Split>();
			this.OriginalDescription = string.Empty;
			this.Description = string.Empty;
		}

		public int Id { get; set; }
		public int AccountId { get; set; }
		public DateTime Date { get; set; }
		public string OriginalDescription { get; set; }
		public string Description { get; set; }
		public bool Pending { get; set; }
		public int DayOrder { get; set; }
		public int ImportState { get; set; }

		public virtual Account Account { get; set; }
		public virtual IList<Split> Splits { get; set; }
	}
}