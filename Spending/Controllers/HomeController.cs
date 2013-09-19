using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Spending.Models;

namespace Spending.Controllers
{
	public class HomeController : Controller
	{
		[Authorize]
		public ActionResult Index()
		{
			HomeModel model = new HomeModel();

			using (var db = new SpendingContext())
			{
				var user = db.Users.Find(Membership.GetUser().ProviderUserKey);

				model.Month = user.Settings.Select(x => x.Month).First();
				model.Month.AddDays(-model.Month.Day + 1);

				model.AccountsInfo = new AccountsModel();

				foreach (var item in user.Accounts.OrderBy(x => x.Order).ToList())
				{
					var itemInfo = new AccountModel();
					itemInfo.Id = item.Id;
					itemInfo.Name = item.Name;
					itemInfo.Starting = item.Transactions.Where(x => !x.Pending && x.Date < model.Month).SelectMany(x => x.Splits).Sum(x => x.Amount);
					itemInfo.Ending = itemInfo.Starting + item.Transactions
						.Where(x => !x.Pending && x.Date >= model.Month && x.Date < model.Month.AddMonths(1))
						.SelectMany(x => x.Splits)
						.Sum(x => x.Amount);

					model.AccountsInfo.Accounts.Add(itemInfo);
					model.AccountsInfo.Starting += itemInfo.Starting;
					model.AccountsInfo.Ending += itemInfo.Ending;
				}
				
				model.CategoriesInfo = new CategoriesModel();

				model.CategoriesInfo.Unassigned = user.Accounts
					.SelectMany(x => x.Transactions)
					.Where(x => x.Date < model.Month)
					.SelectMany(x => x.Splits)
					.Sum(x => x.Amount);

				foreach (var category in user.Categories.Where(x => x.Income).ToList())
				{
					model.CategoriesInfo.Unassigned += category.Splits.Where(x => x.Amount > 0).Sum(x => x.Amount);
				}

				foreach (var group in user.CategoryGroups.OrderBy(x => x.Order).ToList())
				{
					var groupInfo = new CategoryGroupModel();
					groupInfo.Id = group.Id;
					groupInfo.Name = group.Name;

					foreach (var item in group.Categories.Where(x => !x.Income && !x.System).OrderBy(x => x.Order).ToList())
					{
						decimal budgetAmount = 0;

						foreach (var budget in item.Budgets)
						{
							if (budget.Frequency > 0)
							{
								var monthDiff =
									(model.Month.Year - budget.StartingMonth.Year) * 12 +
									model.Month.Month - budget.StartingMonth.Month;

								if (monthDiff >= 0 &&
									(budget.Times == 0 || monthDiff / budget.Frequency < budget.Times) &&
									monthDiff % budget.Frequency == 0)
								{
									budgetAmount += budget.Amount;
								}
							}
						}

						decimal runningBudget = 0;
						decimal targetAvg = 0;
						int targetMonths = 0;
						decimal targetRunningBudget = 0;
						var month = DateTime.UtcNow;

						for (int months = 1; months < 600; months++)
						{
							month = month.AddMonths(1);

							decimal totalBudget = 0;

							foreach (var budget in item.Budgets)
							{
								if (budget.Frequency > 0)
								{
									var monthDiff =
										(month.Year - budget.StartingMonth.Year) * 12 +
										month.Month - budget.StartingMonth.Month;

									if (monthDiff >= 0 &&
										(budget.Times == 0 || monthDiff / budget.Frequency < budget.Times) &&
										monthDiff % budget.Frequency == 0)
									{
										totalBudget += budget.Amount;
									}
								}
							}

							if (totalBudget > 0)
							{
								runningBudget += totalBudget;

								var avg = decimal.Round(totalBudget / months, 2);

								if (avg > targetAvg)
								{
									targetAvg = avg;
									targetMonths = months;
									targetRunningBudget = runningBudget;
								}
							}
						}

						var spent = -item.Splits
							.Where(x => x.Transaction.Date.Month == model.Month.Month && x.Transaction.Date.Year == model.Month.Year)
							.Sum(x => x.Amount);

						var actualBudget = item.EstimateBudget ? budgetAmount : item.Budget;

						var itemInfo = new CategoryModel();
						itemInfo.Id = item.Id;
						itemInfo.Name = item.Name;
						itemInfo.Starting = item.StartingBalance + item.Added;
						itemInfo.Spent = spent;
						itemInfo.Budget = actualBudget;
						itemInfo.Left = actualBudget - spent;
						itemInfo.Ending = itemInfo.Starting - actualBudget;

						var saved = item.StartingBalance - actualBudget;

						itemInfo.Goal = saved + (targetMonths == 0 ? 0 : (targetRunningBudget - saved) / targetMonths);
						itemInfo.SpentPercent = itemInfo.Budget == 0 ? 100 :
							Math.Min(100, (int)(spent / itemInfo.Budget * 100));
						groupInfo.Items.Add(itemInfo);

						groupInfo.Starting += itemInfo.Starting;
						groupInfo.Spent += itemInfo.Spent;
						groupInfo.Budget += itemInfo.Budget;
						groupInfo.Left += itemInfo.Left;
						groupInfo.Ending += itemInfo.Ending;
						groupInfo.Goal += itemInfo.Goal;
						groupInfo.SpentPercent = groupInfo.Budget == 0 ? 100 :
							Math.Min(100, (int)(groupInfo.Spent / groupInfo.Budget * 100));
					}

					model.CategoriesInfo.Groups.Add(groupInfo);

					model.CategoriesInfo.Starting += groupInfo.Starting;
					model.CategoriesInfo.Spent += groupInfo.Spent;
					model.CategoriesInfo.Budget += groupInfo.Budget;
					model.CategoriesInfo.Left += groupInfo.Left;
					model.CategoriesInfo.Ending += groupInfo.Ending;
					model.CategoriesInfo.Goal += groupInfo.Goal;

					model.CategoriesInfo.SpentPercent = model.CategoriesInfo.Budget == 0 ? 100 :
						Math.Min(100, (int)(model.CategoriesInfo.Spent / model.CategoriesInfo.Budget * 100));
				}

				model.CategoriesInfo.Unassigned -= model.CategoriesInfo.Starting;
			}

			return View(model);
		}
	}
}
