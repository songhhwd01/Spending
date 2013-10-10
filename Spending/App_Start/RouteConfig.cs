using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Spending
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
				name: "Budget",
				url: "Budget/{year}/{month}",
				defaults: new { controller = "Home", action = "Index" }
			);

            routes.MapRoute(
				name: "CurrentBudget",
				url: "",
				defaults: new { controller = "Home", action = "Index", year = DateTime.UtcNow.Year, month = DateTime.UtcNow.Month }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}