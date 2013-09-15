using System;
using System.Collections.Generic;

namespace Spending.Models
{
	public class CategoryGroup
	{
		public CategoryGroup()
		{
			this.Categories = new List<Category>();
		}

		public int Id { get; set; }
		public int UserId { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }

		public virtual User User { get; set; }
		public virtual ICollection<Category> Categories { get; set; }
	}
}