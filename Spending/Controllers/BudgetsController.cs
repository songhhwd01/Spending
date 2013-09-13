using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;

namespace Spending.Controllers
{
	public class BudgetsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Index()
		{
			return View(db.Budgets.ToList());
		}

		public ActionResult Create()
		{
			this.ViewBag.Categories = db.Categories;
			return View(new Budget());
		}

		[HttpPost]
		public ActionResult Create(Budget estimate)
		{
			if (ModelState.IsValid)
			{
				db.Budgets.Add(estimate);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(estimate);
		}

		public ActionResult Edit(int id = 0)
		{
			Budget estimate = db.Budgets.Find(id);

			if (estimate == null)
			{
				return HttpNotFound();
			}

			this.ViewBag.Categories = db.Categories;

			return View(estimate);
		}

		[HttpPost]
		public ActionResult Edit(Budget estimate)
		{
			if (ModelState.IsValid)
			{
				db.Budgets.Attach(estimate);
				var entry = db.Entry(estimate);
				entry.Property(x => x.CategoryId).IsModified = true;
				entry.Property(x => x.Amount).IsModified = true;
				entry.Property(x => x.StartingMonth).IsModified = true;
				db.SaveChanges();
				return RedirectToAction("Index", "Home");
			}

			return View(estimate);
		}

		public ActionResult Delete(int id = 0)
		{
			Budget estimate = db.Budgets.Find(id);

			if (estimate == null)
			{
				return HttpNotFound();
			}

			return View(estimate);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			Budget estimate = db.Budgets.Find(id);

			if (estimate == null)
			{
				return HttpNotFound();
			}

			db.Budgets.Remove(estimate);
			db.SaveChanges();
			return RedirectToAction("Index");
		}
	}
}
