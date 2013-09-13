using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class Split
	{
		public Split()
		{
			this.Notes = string.Empty;
		}

		public int Id { get; set; }
		public int TransactionId { get; set; }
		public int CategoryId { get; set; }
		public string Notes { get; set; }
		public decimal Amount { get; set; }

		public virtual Category Category { get; set; }
		public virtual Transaction Transaction { get; set; }
	}
}