using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Spending.Models
{
	public class BoaLogin
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Question1 { get; set; }
		public string Answer1 { get; set; }
		public string Question2 { get; set; }
		public string Answer2 { get; set; }
		public string Question3 { get; set; }
		public string Answer3 { get; set; }
	}
}