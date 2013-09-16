using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Models
{
	public class EditBudgetModel
	{
		public int Id { get; set; }
		public DateTime StartingMonth { get; set; }
		public decimal Amount { get; set; }
		public int Frequency { get; set; }
		public int Times { get; set; }
		public string Notes { get; set; }
		public bool Delete { get; set; }
	}

	public class EditCategoryModel
	{
		public EditCategoryModel()
		{
			this.Budgets = new List<EditBudgetModel>();
			this.NewBudgets = new List<EditBudgetModel>();
		}

		public int Id { get; set; }
		public int GroupId { get; set; }
		public string Name { get; set; }
		public decimal OriginalStartingBalance { get; set; }
		public decimal StartingBalance { get; set; }
		public decimal Budget { get; set; }
		public bool EstimateBudget { get; set; }
		public decimal EstimatedBudget { get; set; }
		public List<EditBudgetModel> Budgets { get; set; }
		public List<EditBudgetModel> NewBudgets { get; set; }
	}

	public class CreateBudgetModel
	{
		public CreateBudgetModel()
		{
			this.NewBudgets = new Dictionary<string, EditBudgetModel>();
		}

		public Dictionary<string, EditBudgetModel> NewBudgets { get; set; }
	}
}