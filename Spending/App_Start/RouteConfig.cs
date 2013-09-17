﻿using System;
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
				name: "Transactions",
				url: "Transactions",
				defaults: new { controller = "Transactions", action = "Index", startDate = DateTime.MinValue.Date,
					endDate = DateTime.MaxValue.Date, transactionsPerPage = 30, page = 1 }
			);
            routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}