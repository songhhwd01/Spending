using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;

namespace Spending.Controllers
{
	[Authorize]
	public class AccountGroupsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Create()
		{
			return View(new AccountGroup());
		}

		[HttpPost]
		public ActionResult Create(AccountGroup group)
		{
			if (ModelState.IsValid)
			{
				db.AccountGroups.Add(group);
				db.AccountSubGroups.Add(new AccountSubGroup { Group = group, Name = group.Name, Order = 0 });

				byte order = 0;

				foreach (var item in db.AccountGroups.OrderBy(x => x.Order))
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
				return RedirectToAction("Index", "Accounts");
			}

			return View(group);
		}

		public ActionResult Edit(int id = 0)
		{
			AccountGroup group = db.AccountGroups.Find(id);

			if (group == null)
			{
				return HttpNotFound();
			}

			return View(group);
		}

		[HttpPost]
		public ActionResult Edit(AccountGroup group)
		{
			if (ModelState.IsValid)
			{
				db.Entry(group).State = EntityState.Modified;

				byte order = 0;

				foreach (var item in db.AccountGroups.OrderBy(x => x.Order))
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

				return RedirectToAction("Index", "Accounts");
			}

			return View(group);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			AccountGroup group = db.AccountGroups.Find(id);

			if (group == null)
			{
				return HttpNotFound();
			}

			if (group.SubGroups.Count == 1)
			{
				db.AccountSubGroups.Remove(group.SubGroups.ElementAt(0));
			}

			db.AccountGroups.Remove(group);
			db.SaveChanges();

			return RedirectToAction("Index", "Accounts");
		}
	}
}
