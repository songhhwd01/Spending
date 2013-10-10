using System;

namespace Spending.Models
{
	public class Setting
	{
		public int Id { get; set; }
		public DateTime Month { get; set; }
		public int Code { get; set; }
		public bool Locked { get; set; }
	}
}