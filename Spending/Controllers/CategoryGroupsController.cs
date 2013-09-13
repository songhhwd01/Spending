using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;

namespace Spending.Controllers
{
	public class CategoryGroupsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Create()
		{
            return View(new CategoryGroup());
		}

		[HttpPost]
		public ActionResult Create(CategoryGroup group)
		{
			if (ModelState.IsValid)
			{
				db.CategoryGroups.Add(group);
	
				byte order = 0;

				foreach (var item in db.CategoryGroups.OrderBy(x => x.Order))
				{
					if (order == group.Order)
					{
						order++;
					}

					if (item != group)
					{
						item.Order = order++;
					}
				}

				db.SaveChanges();

                return RedirectToAction("Index", "Home");
			}

			return View(group);
		}

		public ActionResult Edit(int id = 0)
		{
			CategoryGroup group = db.CategoryGroups.Find(id);

			if (group == null)
			{
				return HttpNotFound();
			}

			return View(group);
		}

		[HttpPost]
		public ActionResult Edit(CategoryGroup group)
		{
			if (ModelState.IsValid)
			{
				db.CategoryGroups.Attach(group);
				var entry = db.Entry(group);
				entry.Property(x => x.Name).IsModified = true;
				entry.Property(x => x.Order).IsModified = true;

				byte order = 0;

				foreach (var item in db.CategoryGroups.OrderBy(x => x.Order))
				{
					if (order == group.Order)
					{
						order++;
					}

					if (item != group)
					{
						item.Order = order++;
					}
				}

				db.SaveChanges();

				return RedirectToAction("Index", "Home");
			}

			return View(group);
		}

		public ActionResult Delete(int id = 0)
		{
			CategoryGroup group = db.CategoryGroups.Find(id);

			if (group == null)
			{
				return HttpNotFound();
			}

			return View(group);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			CategoryGroup group = db.CategoryGroups.Find(id);

			if (group == null)
			{
				return HttpNotFound();
			}

			db.CategoryGroups.Remove(group);
			db.SaveChanges();

			return RedirectToAction("Index", "Home");
		}
	}
}
