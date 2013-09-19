using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Spending.Models;
using Spending.ViewModels;

namespace Spending.Controllers
{
	public class TransactionsController : Controller
	{
		private SpendingContext db = new SpendingContext();

		public ActionResult Index(int accountId = 0, DateTime? startDate = null, DateTime? endDate = null, int transactionsPerPage = 30, int page = 1)
		{
			startDate = startDate ?? DateTime.MinValue.Date;
			endDate = endDate ?? DateTime.MaxValue.Date;

			var model = new TransactionsIndexModel();

			var transactions = db.Transactions
				.Where(x => x.AccountId == accountId && x.Date >= startDate && x.Date <= endDate)
				.OrderByDescending(x => x.Date).ThenByDescending(x => x.DayOrder);

			model.Transactions = transactions.Skip((page - 1) * transactionsPerPage).Take(transactionsPerPage).ToList();

			if (accountId > 0 && startDate.Value.Date == DateTime.MinValue.Date && endDate.Value.Date == DateTime.MaxValue.Date)
			{
				model.Account = db.Accounts.Find(accountId);
			}

			model.StartDate = startDate.Value;
			model.EndDate = endDate.Value;
			model.TransactionsPerPage = transactionsPerPage;
			model.Page = page;
			model.LastPage = (int)Math.Ceiling(transactions.Count() / (double)transactionsPerPage);

			return View(model);
		}

		public ActionResult Create(int accountId)
		{
			this.ViewBag.Accounts = db.Accounts;
			this.ViewBag.Categories = db.Categories;

			EditTransactionModel model = new EditTransactionModel();
			model.Date = DateTime.Now;
			model.AccountId = accountId;
			model.NewSplits.Add(new EditSplitModel());
			return View(model);
		}

		[HttpPost]
		public ActionResult Create(EditTransactionModel model)
		{
			if (ModelState.IsValid)
			{
				Transaction transaction = new Transaction();
				transaction.Date = model.Date;
				transaction.AccountId = model.AccountId;
				transaction.Description = model.Description ?? string.Empty;
				transaction.Pending = model.Pending;
				db.Transactions.Add(transaction);

				foreach (var splitModel in model.NewSplits)
				{
					Split split = new Split();
					split.CategoryId = splitModel.CategoryId;
					split.Notes = splitModel.Notes ?? string.Empty;
					split.Amount = splitModel.Amount;
					transaction.Splits.Add(split);
				}

				db.SaveChanges();
				return RedirectToAction("Index", new { accountId = model.AccountId });
			}

			this.ViewBag.Accounts = db.Accounts;
			this.ViewBag.Categories = db.Categories;
			return View(model);
		}

		public ActionResult Edit(int id)
		{
			Transaction transaction = db.Transactions.Find(id);

			if (transaction == null)
			{
				return HttpNotFound();
			}

			this.ViewBag.Accounts = db.Accounts;
			this.ViewBag.Categories = db.Categories;

			EditTransactionModel model = new EditTransactionModel();
			model.Id = transaction.Id;
			model.Date = transaction.Date;
			model.AccountId = transaction.AccountId;
			model.Description = transaction.Description;
			model.OriginalDescription = transaction.OriginalDescription;
			model.Pending = transaction.Pending;

			foreach (var split in transaction.Splits)
			{
				var splitModel = new EditSplitModel();
				splitModel.Id = split.Id;
				splitModel.CategoryId = split.CategoryId;
				splitModel.Notes = split.Notes;
				splitModel.Amount = split.Amount;
				model.Splits.Add(splitModel);
			}

			return View(model);
		}

		public ActionResult CreateSplit()
		{
			this.ViewBag.Categories = db.Categories;

			var model = new CreateSplitModel();
			model.NewSplits.Add(Guid.NewGuid().ToString(), new EditSplitModel());
			return PartialView(model);
		}

		[HttpPost]
		public ActionResult Edit(EditTransactionModel model)
		{
			if (ModelState.IsValid)
			{
				Transaction transaction = new Transaction();
				transaction.Id = model.Id;
				transaction.Date = model.Date;
				transaction.AccountId = model.AccountId;
				transaction.Description = model.Description ?? string.Empty;
				transaction.Pending = model.Pending;

				db.Transactions.Attach(transaction);

				var transactionEntry = db.Entry(transaction);
				transactionEntry.Property(x => x.Date).IsModified = true;
				transactionEntry.Property(x => x.AccountId).IsModified = true;
				transactionEntry.Property(x => x.Description).IsModified = true;
				transactionEntry.Property(x => x.Pending).IsModified = true;

				foreach (var splitModel in model.Splits)
				{
					if (splitModel.Delete)
					{
						Split split = new Split();
						split.Id = splitModel.Id;
						db.Splits.Attach(split);
						db.Splits.Remove(split);
					}
					else
					{
						Split split = new Split();
						split.Id = splitModel.Id;
						split.TransactionId = transaction.Id;
						split.CategoryId = splitModel.CategoryId;
						split.Notes = splitModel.Notes ?? string.Empty;
						split.Amount = splitModel.Amount;

						db.Splits.Attach(split);

						var splitEntry = db.Entry(split);
						splitEntry.Property(x => x.CategoryId).IsModified = true;
						splitEntry.Property(x => x.Notes).IsModified = true;
						splitEntry.Property(x => x.Amount).IsModified = true;
					}
				}

				foreach (var splitModel in model.NewSplits)
				{
					Split split = new Split();
					split.TransactionId = transaction.Id;
					split.CategoryId = splitModel.CategoryId;
					split.Notes = splitModel.Notes ?? string.Empty;
					split.Amount = splitModel.Amount;
					db.Splits.Add(split);
				}

				db.SaveChanges();
				return RedirectToAction("Index", new { accountId = model.AccountId });
			}

			this.ViewBag.Accounts = db.Accounts;
			this.ViewBag.Categories = db.Categories;
			return View(model);
		}

		[HttpPost]
		public ActionResult UpdateDayOrder(int id, int dayOrder)
		{
			var transaction = db.Transactions.Find(id);

			if (transaction == null)
			{
				return HttpNotFound();
			}

			transaction.DayOrder = dayOrder;
			db.Transactions.Attach(transaction);
			db.Entry(transaction).Property(x => x.DayOrder).IsModified = true;

			byte currOrder = 0;

			foreach (var item in db.Transactions.Where(x => x.Date == transaction.Date).OrderBy(x => x.DayOrder))
			{
				if (currOrder == dayOrder)
				{
					currOrder++;
				}

				if (item != transaction)
				{
					item.DayOrder = currOrder++;
				}
			}

			db.SaveChanges();

			return RedirectToAction("Details", "Accounts", new { id = transaction.AccountId });
		}

		[HttpPost]
		public ActionResult Delete(int id, int accountId)
		{
			Transaction transaction = db.Transactions.Find(id);
			
			if (transaction == null)
			{
				return HttpNotFound();
			}

			db.Transactions.Remove(transaction);
			db.SaveChanges();
			return RedirectToAction("Details", "Accounts", new { id = accountId });
		}
	}
}
