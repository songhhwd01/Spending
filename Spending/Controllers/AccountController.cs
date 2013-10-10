using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Spending.Models;

namespace Spending.Controllers
{
	public class AccountController : Controller
	{
		[AllowAnonymous]
		public ActionResult Unlock()
		{
			this.ViewBag.IncorrectCode = false;
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult Unlock(int code)
		{
			using (var db = new SpendingContext())
			{
				var storedCode = db.Settings.Select(x => x.Code).First();

				if (code == storedCode)
				{
					db.Settings.First().Locked = false;
					db.SaveChanges();

					return RedirectToAction("Index", "Home");
				}
			}

			this.ViewBag.IncorrectCode = true;
			return View();
		}

		public ActionResult Lock()
		{
			using (var db = new SpendingContext())
			{
				var settings = db.Settings.First();
				settings.Locked = true;
				db.SaveChanges();

				return RedirectToAction("Unlock");
			}
		}

		public ActionResult ChangePassword()
		{
			return View();
		}

		[HttpPost]
		public ActionResult ChangePassword(AccountChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{
				bool success;

				try
				{
					MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
					success = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
				}
				catch (Exception)
				{
					success = false;
				}

				if (success)
				{
					return RedirectToAction("ChangePasswordSuccess");
				}

				ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
			}

			return View(model);
		}

		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}

		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
	}
}
