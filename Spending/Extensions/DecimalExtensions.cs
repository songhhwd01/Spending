using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Extensions
{
	public static class DecimalExtensions
	{
		public static string ToCurrencyString(this decimal value)
		{
			return value.ToString("#,##0.00");
		}
	}
}