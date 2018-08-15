using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GoldAlliance.Models;
using System.Text.RegularExpressions;

namespace GoldAlliance.Controllers
{
    public class AccountsController : Controller
    {
        private GoldAllianceEntities db = new GoldAllianceEntities();

        // GET: Accounts
        public ActionResult Index()
        {
            var memberId = Session["MemberId"];
            if (memberId != null)
            {
                var accounts = db.Accounts
                    .Include(a => a.AccountStatus)
                    .Include(a => a.AccountType)
                    .Include(a => a.Member)
                    .Where(a => a.MemberId == (int)memberId && a.AccountStatusCode.Equals("A"))
                    .ToList<Account>();

                return View(accounts.ToList());
            }
            else
            {
                ModelState.AddModelError("", "Member does not exist");
                return View();
            }
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            //ViewBag.AccountStatusCode = new SelectList(db.AccountStatuses, "AccountStatusCode", "Description");
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "AccountTypeId", "AccountType1");
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountNumber,MemberId,AccountTypeId,CurrentBalance,OpenDate,AccountStatusCode,Overdraft")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.MemberId = (int) Session["MemberId"];

                //generate unique 12 digit account number and store it
                int accountNumber = generateAccountNumber();
                account.AccountNumber = accountNumber;

                //when account is created current balance is 0.00, status is active, overdraft is false
                account.CurrentBalance = (decimal)0;
                account.AccountStatusCode = "A";
                account.Overdraft = false;
                account.OpenDate = DateTime.Now;

                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AccountStatusCode = new SelectList(db.AccountStatuses, "AccountStatusCode", "Description", account.AccountStatusCode);
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "AccountTypeId", "AccountType1", account.AccountTypeId);
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", account.MemberId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AccountStatusCode = new SelectList(db.AccountStatuses, "AccountStatusCode", "Description", account.AccountStatusCode);
            //ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "AccountTypeId", "AccountType1", account.AccountTypeId);
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", account.MemberId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountNumber,MemberId,AccountTypeId,CurrentBalance,OpenDate,AccountStatusCode,Overdraft")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountStatusCode = new SelectList(db.AccountStatuses, "AccountStatusCode", "Description", account.AccountStatusCode);
            ViewBag.AccountTypeId = new SelectList(db.AccountTypes, "AccountTypeId", "AccountType1", account.AccountTypeId);
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", account.MemberId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            //db.Accounts.Remove(account);

            //if account has +/- balance, cannot close
            if(account.CurrentBalance != (decimal)0)
            {
                ModelState.AddModelError("", "Cannot close account with positive/negative balance");
                return View(account);
            }

            //If balance = 0, make account inactive
            account.AccountStatusCode = "I";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CalculateInterest(AccountInterestViewModel model, int id)
        {
            Account account = db.Accounts.Find(id);

            //check if account is business
            if(account.AccountTypeId == 2)
            {
                return View("BusinessError");
            }

            var Principal = (double) account.CurrentBalance;
            var InterestRate = (double) account.AccountType.InterestRate / 100;
            var Interest = Principal * InterestRate * 1;

            model.InterestRate = (decimal) (InterestRate * 100);
            model.AccountInterest = (decimal) Interest;

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private int generateAccountNumber()
        {
            var temp = Guid.NewGuid().ToString().Replace("-", "");
            var accountNo = Regex.Replace(temp, "[a-zA-Z]", "").Substring(0, 8);

            return Convert.ToInt32(accountNo);
        }
    }
}
