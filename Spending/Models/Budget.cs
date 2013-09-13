using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class Budget
	{
		public Budget()
		{
		}

		public int Id { get; set; }
		public int CategoryId { get; set; }
		public decimal Amount { get; set; }
		public DateTime StartingMonth { get; set; }
		public int Frequency { get; set; }
		public int Times { get; set; }
		public string Notes { get; set; }

		public virtual Category Category { get; set; }
	}
}