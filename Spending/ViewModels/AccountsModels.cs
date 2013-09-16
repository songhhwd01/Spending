using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spending.Models
{
	public enum ImportState
	{
		Manual,
		Added,
		Removed,
		Matched,
		Reconciled
	}
}