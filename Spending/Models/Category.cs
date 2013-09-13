using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class Category
	{
		public Category()
		{
			this.Budgets = new List<Budget>();
			this.Splits = new List<Split>();
		}

		public int Id { get; set; }
		public int GroupId { get; set; }
		public string Name { get; set; }
		public decimal StartingBalance { get; set; }
		public decimal Added { get; set; }
		public decimal Budget { get; set; }
		public bool EstimateBudget { get; set; }
		public bool Income { get; set; }
		public bool System { get; set; }
		public int Order { get; set; }

		public virtual CategoryGroup Group { get; set; }
		public virtual ICollection<Budget> Budgets { get; set; }
		public virtual ICollection<Split> Splits { get; set; }
	}
}