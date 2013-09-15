using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Spending.Models;

namespace Spending.Controllers
{
	[Authorize]
	public class CategoriesController : Controller
	{
		private SpendingContext db = new SpendingContext();

		[HttpPost]
		public ActionResult UpdateOrder(int id, int groupId, int order)
		{
			var category = db.Categories.Find(id);

			if (category == null)
			{
				return HttpNotFound();
			}

			category.GroupId = groupId;
			category.Order = order;

			db.Categories.Attach(category);

			var categoryEntry = db.Entry(category);
			categoryEntry.Property(x => x.GroupId).IsModified = true;
			categoryEntry.Property(x => x.Order).IsModified = true;

			byte currOrder = 0;

			foreach (var item in db.Categories.Where(x => !x.Income && !x.System && x.GroupId == groupId).OrderBy(x => x.Order))
			{
				if (currOrder == order)
				{
					currOrder++;
				}

				if (item != category)
				{
					item.Order = currOrder++;
				}
			}

			db.SaveChanges();

			return new EmptyResult();
		}

		[HttpPost]
		public ActionResult UpdateStartingBalance(int id, decimal value)
		{
			var category = new Category();
			category.Id = id;
			category.StartingBalance = value;

			db.Categories.Attach(category);

			var categoryEntry = db.Entry(category);
			categoryEntry.Property(x => x.StartingBalance).IsModified = true;

			db.SaveChanges();

			return new EmptyResult();
		}

		public ActionResult Create()
		{
			this.ViewBag.Groups = db.CategoryGroups;

			EditCategoryModel model = new EditCategoryModel();
			model.NewBudgets.Add(new EditBudgetModel());
			return View(model);
		}

		[HttpPost]
		public ActionResult Create(EditCategoryModel model)
		{
			if (ModelState.IsValid)
			{
				var category = new Category();
				category.Name = model.Name;
				category.GroupId = model.GroupId;
				category.StartingBalance = model.OriginalStartingBalance;
				category.Added = model.StartingBalance - model.OriginalStartingBalance;
				category.Budget = model.Budget;
				db.Categories.Add(category);

				foreach (var budgetModel in model.NewBudgets)
				{
					var budget = new Budget();
					budget.Amount = budgetModel.Amount;
					budget.StartingMonth = budgetModel.StartingMonth;
					budget.Frequency = budgetModel.Frequency;
					budget.Times = budgetModel.Times;
					budget.Notes = budgetModel.Notes ?? string.Empty;
					category.Budgets.Add(budget);
				}

				db.SaveChanges();
				return RedirectToAction("Index", "Home");
			}

			this.ViewBag.Groups = db.CategoryGroups;
			return View(model);
		}

		public ActionResult Edit(int id = 0)
		{
			var category = db.Categories.Find(id);

			if (category == null)
			{
				return HttpNotFound();
			}

			var user = db.Users.Find(Membership.GetUser().ProviderUserKey);

			var currentMonth = user.Settings.Select(x => x.Month).First();

			EditCategoryModel model = new EditCategoryModel();
			model.Id = category.Id;
			model.Name = category.Name;
			model.GroupId = category.GroupId;
			model.OriginalStartingBalance = category.StartingBalance;
			model.StartingBalance = category.StartingBalance + category.Added;
			model.Budget = category.Budget;
			model.EstimateBudget = category.EstimateBudget;

			foreach (var budget in category.Budgets)
			{
				if (budget.Frequency > 0)
				{
					var monthDiff =
						(currentMonth.Year - budget.StartingMonth.Year) * 12 +
						currentMonth.Month - budget.StartingMonth.Month;

					if (monthDiff >= 0 &&
						(budget.Times == 0 || monthDiff / budget.Frequency < budget.Times) &&
						monthDiff % budget.Frequency == 0)
					{
						model.EstimatedBudget += budget.Amount;
					}
				}
			}

			foreach (var budget in category.Budgets)
			{
				var budgetModel = new EditBudgetModel();
				budgetModel.Id = budget.Id;
				budgetModel.Amount = budget.Amount;
				budgetModel.StartingMonth = budget.StartingMonth;
				budgetModel.Frequency = budget.Frequency;
				budgetModel.Times = budget.Times;
				budgetModel.Notes = budget.Notes;
				model.Budgets.Add(budgetModel);
			}

			return View(model);
		}

		public ActionResult CreateBudget()
		{
			var model = new CreateBudgetModel();
			model.NewBudgets.Add(Guid.NewGuid().ToString(), new EditBudgetModel());
			return PartialView(model);
		}

		[HttpPost]
		public ActionResult Edit(EditCategoryModel model)
		{
			if (ModelState.IsValid)
			{
				var category = new Category();
				category.Id = model.Id;
				category.Name = model.Name;
				category.GroupId = model.GroupId;
				category.Added = model.StartingBalance - model.OriginalStartingBalance;
				category.Budget = model.Budget;
				category.EstimateBudget = model.EstimateBudget;

				db.Categories.Attach(category);

				var categoryEntry = db.Entry(category);
				categoryEntry.Property(x => x.Name).IsModified = true;
				categoryEntry.Property(x => x.GroupId).IsModified = true;
				categoryEntry.Property(x => x.Added).IsModified = true;
				categoryEntry.Property(x => x.Budget).IsModified = true;
				categoryEntry.Property(x => x.EstimateBudget).IsModified = true;

				foreach (var budgetModel in model.Budgets)
				{
					if (budgetModel.Delete)
					{
						Budget budget = new Budget();
						budget.Id = budgetModel.Id;
						db.Budgets.Attach(budget);
						db.Budgets.Remove(budget);
					}
					else
					{
						Budget budget = new Budget();
						budget.Id = budgetModel.Id;
						budget.Amount = budgetModel.Amount;
						budget.StartingMonth = budgetModel.StartingMonth;
						budget.Frequency = budgetModel.Frequency;
						budget.Times = budgetModel.Times;
						budget.Notes = budgetModel.Notes ?? string.Empty;

						db.Budgets.Attach(budget);

						var budgetEntry = db.Entry(budget);
						budgetEntry.Property(x => x.Amount).IsModified = true;
						budgetEntry.Property(x => x.StartingMonth).IsModified = true;
						budgetEntry.Property(x => x.Frequency).IsModified = true;
						budgetEntry.Property(x => x.Times).IsModified = true;
						budgetEntry.Property(x => x.Notes).IsModified = true;
					}
				}

				foreach (var budgetModel in model.NewBudgets)
				{
					Budget budget = new Budget();
					budget.CategoryId = category.Id;
					budget.Amount = budgetModel.Amount;
					budget.StartingMonth = budgetModel.StartingMonth;
					budget.Frequency = budgetModel.Frequency;
					budget.Times = budgetModel.Times;
					budget.Notes = budgetModel.Notes ?? string.Empty;
					db.Budgets.Add(budget);
				}
	
				db.SaveChanges();
				return RedirectToAction("Index", "Home");
			}

			this.ViewBag.Groups = db.CategoryGroups;
			return View(model);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			Category category = db.Categories.Find(id);

			if (category == null)
			{
				return HttpNotFound();
			}

			db.Categories.Remove(category);
			db.SaveChanges();
			return RedirectToAction("Index", "Home");
		}
	}
}
