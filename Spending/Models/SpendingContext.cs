using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Spending.Models
{
	public class SpendingContext : DbContext
	{
		static SpendingContext()
		{
			Database.SetInitializer<SpendingContext>(null);
		}

		public SpendingContext()
		{
		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<BoaLogin> BoaLogins { get; set; }
		public DbSet<Budget> Budgets { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<CategoryGroup> CategoryGroups { get; set; }
		public DbSet<Setting> Settings { get; set; }
		public DbSet<Split> Splits { get; set; }
		public DbSet<Transaction> Transactions { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
		}
	}
}
