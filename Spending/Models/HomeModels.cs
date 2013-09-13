using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Models
{
	public class HomeModel
	{
		public HomeModel()
		{
			this.Groups = new List<CategoryGroupModel>();
		}

		public DateTime Month { get; set; }
		public decimal Unassigned { get; set; }
		public decimal Starting { get; set; }
		public decimal Spent { get; set; }
		public decimal Budget { get; set; }
		public decimal Left { get; set; }
		public decimal Ending { get; set; }
		public decimal Goal { get; set; }
		public int SpentPercent { get; set; }
		public List<CategoryGroupModel> Groups { get; set; }
	}

	public class CategoryGroupModel
	{
		public CategoryGroupModel()
		{
			this.Items = new List<CategoryModel>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Starting { get; set; }
		public decimal Spent { get; set; }
		public decimal Budget { get; set; }
		public decimal Left { get; set; }
		public decimal Ending { get; set; }
		public decimal Goal { get; set; }
		public int SpentPercent { get; set; }
		public List<CategoryModel> Items { get; set; }
	}

	public class CategoryModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Starting { get; set; }
		public decimal Spent { get; set; }
		public decimal Budget { get; set; }
		public decimal Left { get; set; }
		public decimal Ending { get; set; }
		public decimal Goal { get; set; }
		public int SpentPercent { get; set; }
	}
}