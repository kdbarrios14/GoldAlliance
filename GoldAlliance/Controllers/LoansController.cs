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
    public class LoansController : Controller
    {
        private GoldAllianceEntities db = new GoldAllianceEntities();

        // GET: Loans
        public ActionResult Index()
        {
            var memberId = Session["MemberId"];
            if (memberId != null)
            {
                var loans = db.Loans
                    .Include(l => l.LoanType)
                    .Include(l => l.Member)
                    .Where(l => l.MemberId == (int)memberId)
                    .ToList<Loan>();

                return View(loans.ToList());
            }
            else
            {
                ModelState.AddModelError("", "Member does not exist");
                return View();
            }
        }

        // GET: Loans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: Loans/Create
        public ActionResult Create()
        {
            ViewBag.LoanTypeId = new SelectList(db.LoanTypes, "LoanTypeId", "LoanName");
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LoanId,MemberId,LoanTypeId,IssueDate,Amount,Term_Years,PaymentAmount,PaymentDate")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                loan.MemberId = (int)Session["MemberId"];

                loan.IssueDate = DateTime.Now;
                loan.PaymentAmount = (decimal) 0;

                //generate payment date 30 days later
                DateTime paymentDate = generatePayment(loan.IssueDate);
                loan.PaymentDate = paymentDate;

                db.Loans.Add(loan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LoanTypeId = new SelectList(db.LoanTypes, "LoanTypeId", "LoanName", loan.LoanTypeId);
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", loan.MemberId);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            ViewBag.LoanTypeId = new SelectList(db.LoanTypes, "LoanTypeId", "LoanName", loan.LoanTypeId);
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", loan.MemberId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LoanId,MemberId,LoanTypeId,IssueDate,Amount,PaymentAmount,PaymentDate")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LoanTypeId = new SelectList(db.LoanTypes, "LoanTypeId", "LoanName", loan.LoanTypeId);
            ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", loan.MemberId);
            return View(loan);
        }

        public ActionResult CalculatePayments(PaymentViewModel model, int id)
        {
            Loan loan = db.Loans.Find(id);
            var loanType = loan.LoanType;
            var interest = loanType.InterestRate;
            var numberOfYears = loan.Term_Years;
            var amount = (double) loan.Amount;

            var ratePerPeriod = (double) interest / 1200;  //annual interest rate / noMonths / 100
            var numberOfPayments = numberOfYears * 12;

            //Calculate Payment amount with Loan Payment Formula: Payment = rP/(1-(1+r)^-t)
            var paymentAmount = (ratePerPeriod * amount) / (1 - Math.Pow(1 + ratePerPeriod, numberOfPayments * -1));
            var totalInterestPaid = (numberOfPayments * paymentAmount) - amount;

            model.InterestRate = (decimal)interest;
            model.Term = numberOfYears;
            model.MonthlyPayments = (decimal) paymentAmount;
            model.TotalInterestPaid = (decimal) totalInterestPaid;

            return View(model);
        }

        public ActionResult MakePayment(LoanPaymentViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                Loan loan = db.Loans.Find(id);

                var Amount = model.PaymentMade;

                var nextPaymentDate = loan.PaymentDate;

                if(loan.PaymentAmount == null)
                {
                    loan.PaymentAmount = (decimal)0;
                }

                loan.PaymentAmount += Amount;
                loan.PaymentDate = generatePayment(nextPaymentDate);

                db.Entry(loan).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

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

        private DateTime generatePayment(DateTime start)
        {
            DateTime dueDate = start.AddDays(30);

            return dueDate;
        }
    }
}
