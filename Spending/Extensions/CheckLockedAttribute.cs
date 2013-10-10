using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;

namespace Spending
{
	public class CheckLockedAttribute : AuthorizeAttribute
    {
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Request.Url.AbsolutePath != "/Account/Unlock")
			{
				using (var db = new SpendingContext())
				{
					if (db.Settings.Select(x => x.Locked).First())
					{
						filterContext.Result = new RedirectResult("/Account/Unlock");
					}
				}
			}
		}
	}
}