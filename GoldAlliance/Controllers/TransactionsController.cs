using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoldAlliance.Models;

namespace GoldAlliance.Controllers
{
    public class TransactionsController : Controller
    {
        private GoldAllianceEntities db = new GoldAllianceEntities();

        // GET: Transactions
        public ActionResult Index(int id)
        {          
            var transactions = db.Transactions
                .Include(t => t.Account)
                .Include(t => t.TransactionType)
                .Where(t => t.AccountNumber == id)
                .ToList<Transaction>();

            return View(transactions.ToList());
            
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionId,TransTypeId,AccountNumber,AccountForTransfer,TransactionDate,Amount")] Transaction transaction, int id)
        {
            if (ModelState.IsValid)
            {
                transaction.TransactionDate = DateTime.Now;
                transaction.AccountNumber = id;               
                var account = db.Accounts.Find(transaction.AccountNumber);

                switch (transaction.TransTypeId)
                {
                    case 1: //Deposit
                        account.CurrentBalance += transaction.Amount;

                        //if account has overdraft, check if balance is positive to remove overdraft flag
                        if(account.Overdraft && account.CurrentBalance >= 0)
                        {
                            account.Overdraft = false;
                        }
                        break;
                    case 2: //Withdrawal                       
                        if (account.AccountType.AccountType1.Equals("Checking") && transaction.Amount > account.CurrentBalance)
                        {
                            //cannot make transaction
                            ModelState.AddModelError("", "Not enough money in current balance");
                            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
                            return View(transaction);
                        }
                        else if (account.AccountType.AccountType1.Equals("Business") && transaction.Amount > account.CurrentBalance)
                        {
                            //overdraft fee -> "loan"?
                            if (account.Overdraft)
                            {
                                ModelState.AddModelError("", "You already have overdraft!!");
                                ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
                                return View(transaction);
                            }
                            else
                            {
                                account.CurrentBalance -= transaction.Amount;

                                //check if balance is negative
                                decimal overdraftFee = 35;
                                if(account.CurrentBalance < 0)
                                {
                                    account.Overdraft = true;

                                    //apply overdraft fee
                                    account.CurrentBalance -= overdraftFee;
                                }
                            }
                        }
                        else
                        {
                            //make withdrawal
                            account.CurrentBalance -= transaction.Amount;
                        }
                        break;
                    case 3: //Transfer
                        //find transfer account
                        var ToAccount = db.Accounts.Find(transaction.AccountForTransfer);
                        //check if it is same account
                        if (ToAccount.AccountNumber == account.AccountNumber)
                        {
                            //cannot transfer to same account!
                            ModelState.AddModelError("", "Cannot transfer to same account!");
                            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
                            return View(transaction);
                        }
                        //check if there is enough money in primary account
                        if(transaction.Amount > account.CurrentBalance)
                        {
                            ModelState.AddModelError("", "Not enough money to transfer");
                            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
                            return View(transaction);
                        }

                        //make transactions to both accounts
                        account.CurrentBalance -= transaction.Amount;
                        ToAccount.CurrentBalance += transaction.Amount;
                        break;
                }
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index", "Accounts");
            }

            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountNumber = new SelectList(db.Accounts, "AccountNumber", "AccountStatusCode", transaction.AccountNumber);
            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionId,TransTypeId,AccountNumber,AccountForTransfer,TransactionDate,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountNumber = new SelectList(db.Accounts, "AccountNumber", "AccountStatusCode", transaction.AccountNumber);
            ViewBag.TransTypeId = new SelectList(db.TransactionTypes, "TransTypeId", "TransTypeName", transaction.TransTypeId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
