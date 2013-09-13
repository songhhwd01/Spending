using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Extensions
{
	public static class DateTimeExtensions
	{
		public static string ToUrlString(this DateTime dateTime)
		{
			return dateTime.ToString("MM-dd-yyyy");
		}
	}
}