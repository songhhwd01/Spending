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
	public class AccountSubGroupsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Create(int groupId)
		{
            return View(new AccountSubGroup { GroupId = groupId });
		}

		[HttpPost]
		public ActionResult Create(AccountSubGroup subGroup)
		{
			if (ModelState.IsValid)
			{
				db.AccountSubGroups.Add(subGroup);

				byte order = 0;

				foreach (var item in db.AccountGroups.Find(subGroup.GroupId).SubGroups.OrderBy(x => x.Order))
				{
					if (order == subGroup.Order)
					{
						order++;
					}

					if (item != subGroup)
					{
						item.Order = order++;
					}
				}
	
				db.SaveChanges();

                return RedirectToAction("Index", "Accounts");
			}

			return View(subGroup);
		}

		public ActionResult Edit(int id = 0)
		{
			AccountSubGroup subGroup = db.AccountSubGroups.Find(id);

			if (subGroup == null)
			{
				return HttpNotFound();
			}

			return View(subGroup);
		}

		[HttpPost]
		public ActionResult Edit(AccountSubGroup subGroup)
		{
			if (ModelState.IsValid)
			{
				db.AccountSubGroups.Attach(subGroup);
				var entry = db.Entry(subGroup);
				entry.Property(x => x.Name).IsModified = true;
				entry.Property(x => x.Order).IsModified = true;

				byte order = 0;

				foreach (var item in db.AccountGroups.Find(subGroup.GroupId).SubGroups.OrderBy(x => x.Order))
				{
					if (order == subGroup.Order)
					{
						order++;
					}

					if (item != subGroup)
					{
						item.Order = order++;
					}
				}

				db.SaveChanges();

				return RedirectToAction("Index", "Accounts");
			}

			return View(subGroup);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			AccountSubGroup subGroup = db.AccountSubGroups.Find(id);

			if (subGroup == null)
			{
				return HttpNotFound();
			}

			db.AccountSubGroups.Remove(subGroup);
			db.SaveChanges();

			return RedirectToAction("Index", "Accounts");
		}
	}
}
