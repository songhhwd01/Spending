using System;

namespace Spending.Models
{
	public class Setting
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public DateTime Month { get; set; }
	
		public virtual User User { get; set; }
	}
}