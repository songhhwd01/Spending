using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Spending.Models;
using Spending.ViewModels;

namespace Spending.Controllers
{
	public class BoaLoginsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Edit(int id)
		{
			var boaLogin = db.BoaLogins.Find(id);

			if (boaLogin == null)
			{
				return HttpNotFound();
			}

			var model = new BoaLoginsEditModel();
			model.BoaLogin = boaLogin;
			model.AccountsInfo = new AccountsModel();

			foreach (var account in db.Accounts.OrderBy(x => x.Order).ToList())
			{
				var accountInfo = new AccountModel();
				accountInfo.Id = account.Id;
				accountInfo.BoaLogin = account.BoaAccountRefNum == null ? null : db.BoaLogins.FirstOrDefault();
				accountInfo.Name = account.Name;
				accountInfo.Ending = account.Transactions.Where(x => !x.Pending).SelectMany(x => x.Splits).Sum(x => x.Amount);

				model.AccountsInfo.Accounts.Add(accountInfo);
				model.AccountsInfo.Ending += accountInfo.Ending;
			}

			return View(model);
		}
	}
}
