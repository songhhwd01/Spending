using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spending.Models;

namespace Spending.ViewModels
{
	public class BoaLoginsEditModel
	{
		public AccountsModel AccountsInfo { get; set; }
		public BoaLogin BoaLogin { get; set; }
	}
}