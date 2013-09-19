using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;

namespace Spending.Controllers
{
	public class AccountsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult ImportTransactions(int id)
		{
			var account = db.Accounts.Find(id);

			if (account == null)
			{
				return HttpNotFound();
			}

			var accountRefNum = db.Accounts.Where(x => x.BoaAccountRefNum != null).Select(x => x.BoaAccountRefNum).First();
			var boaLogin = db.BoaLogins.First();
			var answers = new Dictionary<string, string>();
			answers.Add(boaLogin.Question1, boaLogin.Answer1);
			answers.Add(boaLogin.Question2, boaLogin.Answer2);
			answers.Add(boaLogin.Question3, boaLogin.Answer3);
			var importedTransactions = BoaTransactionImporter.Import(
				boaLogin.UserName, boaLogin.Password, answers, accountRefNum, true, DateTime.UtcNow.AddMonths(-1), DateTime.MaxValue);

			var uncategorized = db.Categories.Find(37);
			var transactions = account.Transactions.Where(x => x.Date >= importedTransactions.Last().Date).ToList();
			int order = 0;
			DateTime prevDate = DateTime.MinValue;

			foreach (var importedTransaction in ((IEnumerable<BoaTransaction>)importedTransactions).Reverse())
			{
				if (importedTransaction.Date != prevDate)
				{
					prevDate = importedTransaction.Date;
					order = 0;
				}

				var sameAmountTransactions = transactions
					.Where(x => x.Splits.Sum(s => s.Amount) == importedTransaction.Amount);
				var sameAmountAndDateTransactions = sameAmountTransactions
					.Where(x => x.Pending ? importedTransaction.Pending : x.Date == importedTransaction.Date);
				Transaction bestMatch = this.FindBestMatch(importedTransaction, sameAmountAndDateTransactions);

				if (bestMatch == null)
				{
					if (!importedTransaction.Pending)
					{
						var sameAmountAndPendingTransactions = sameAmountTransactions.Where(x => x.Pending);

						if (sameAmountAndPendingTransactions.Count() == 1)
						{
							bestMatch = sameAmountAndPendingTransactions.First();
						}
						else
						{
//							bestMatch = this.FindBestMatch(importedTransaction, sameAmountAndPendingTransactions);
						}
					}
				}

				if (bestMatch == null)
				{
					var transaction = new Transaction();
					transaction.ImportState = (int)ImportState.Added;
					transaction.Pending = importedTransaction.Pending;
					transaction.Date = importedTransaction.Date;
					transaction.OriginalDescription = importedTransaction.Description;
					transaction.DayOrder = order;
					transaction.Splits.Add(new Split { Amount = importedTransaction.Amount, Category = uncategorized });
					account.Transactions.Add(transaction);
				}
				else
				{
					bestMatch.ImportState = (int)ImportState.Matched;
					bestMatch.OriginalDescription = importedTransaction.Description;
					bestMatch.DayOrder = order;
					transactions.Remove(bestMatch);
				}

				order++;
			}

			// The rest of the transactions were not matched, so marking them as unreconciled
			foreach (var transaction in transactions)
			{
				transaction.ImportState = (int)ImportState.Removed;
//				transaction.OriginalDescription = string.Empty;
				transaction.DayOrder = 255;
			}

			db.SaveChanges();

			return RedirectToAction("Details", new { id = account.Id });
		}

		public ActionResult Create()
		{
			return View(new Account());
		}

		[HttpPost]
		public ActionResult Create(Account account)
		{
			if (ModelState.IsValid)
			{
				db.Accounts.Add(account);
				db.SaveChanges();

				return RedirectToAction("Index");
			}

			return View(account);
		}

		public ActionResult Edit(int id = 0)
		{
			Account account = db.Accounts.Find(id);

			if (account == null)
			{
				return HttpNotFound();
			}

			return View(account);
		}

		[HttpPost]
		public ActionResult Edit(Account account)
		{
			if (ModelState.IsValid)
			{
				db.Accounts.Attach(account);
				var entry = db.Entry(account);
				entry.Property(x => x.Name).IsModified = true;

				db.SaveChanges();

				return RedirectToAction("Index");
			}

			return View(account);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id = 0)
		{
			Account account = db.Accounts.Find(id);

			if (account == null)
			{
				return HttpNotFound();
			}

			db.Accounts.Remove(account);
			db.SaveChanges();

			return RedirectToAction("Index");
		}

		private Transaction FindBestMatch(BoaTransaction importedTransaction, IEnumerable<Transaction> transactions)
		{
			int bestLcs = -1;
			Transaction bestMatch = null;

			foreach (var transaction in transactions)
			{
				if (transaction.OriginalDescription == importedTransaction.Description)
				{
					bestMatch = transaction;
					break;
				}

				int lcs = LongestCommonSubsequence(
					transaction.Description.ToUpper(), importedTransaction.Description.ToUpper(), 0, 0);

				if (lcs > bestLcs)
				{
					bestLcs = lcs;
					bestMatch = transaction;
				}
			}

			return bestMatch;
		}

		private static int LongestCommonSubsequence(string strA, string strB, int indexA, int indexB)
		{
			if (indexA == strA.Length || indexB == strB.Length)
			{
				return 0;
			}

			int max = 0;

			for (int i = indexA; i < strA.Length ; i++)
			{
				int exist = strB.IndexOf(strA[i], indexB);

				if (exist != -1)
				{
					int temp = 1 + LongestCommonSubsequence(strA, strB, i + 1, exist + 1);

					if (max < temp)
					{
						max = temp;
					}
				}
			}

			return max;
		}
	}
}
