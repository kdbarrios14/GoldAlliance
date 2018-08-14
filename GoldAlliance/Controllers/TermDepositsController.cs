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
    public class TermDepositsController : Controller
    {
        private GoldAllianceEntities db = new GoldAllianceEntities();

        // GET: TermDeposits
        public ActionResult Index()
        {
            var memberId = Session["MemberId"];
            if (memberId != null)
            {
                var termDeposits = db.TermDeposits
                    .Include(t => t.Member)
                    .Where(t => t.MemberId == (int)memberId)
                    .ToList<TermDeposit>();
                return View(termDeposits.ToList());
            }
            else
            {
                ModelState.AddModelError("", "Member does not exist");
                return View();
            }
        }

        // GET: TermDeposits/Create
        public ActionResult Create()
        {
            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName");
            return View();
        }

        // POST: TermDeposits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TermDepositId,MemberId,Amount,Duration_Months,StartDate,EndDate")] TermDeposit termDeposit)
        {
            if (ModelState.IsValid)
            {
                termDeposit.MemberId = (int)Session["MemberId"];
                termDeposit.StartDate = DateTime.Now;
                var start = termDeposit.StartDate;

                //generate maturity date based on duration and start date
                termDeposit.EndDate = start.AddMonths(termDeposit.Duration_Months);

                db.TermDeposits.Add(termDeposit);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.MemberId = new SelectList(db.Members, "MemberId", "FirstName", termDeposit.MemberId);
            return View(termDeposit);
        }

        public ActionResult CalculateTotalInterest(TdInterestViewModel model, int id)
        {
            TermDeposit td = db.TermDeposits.Find(id);
            var principal = (double) td.Amount;
            var duration = td.Duration_Months;
            var interest = 0.0;
            if(duration < 12)
            {
                interest = 1.25;
            }
            else if(duration < 24)
            {
                interest = 2.05;
            }
            else if(duration < 36)
            {
                interest = 2.25;
            }
            else if(duration < 48)
            {
                interest = 2.35;
            }
            else if(duration < 60)
            {
                interest = 2.5;
            }
            else if(duration < 72)
            {
                interest = 2.95;
            }
            else
            {
                interest = 3.00;
            }

            var rateOfInterest = interest / 100;
            var years = duration / 12;

            //Calculate total with simple interest formula: Total = P(1+rt)
            var totalAmount = principal * (1 + (rateOfInterest * years));
            //Calculate amount earned from interest
            var interestEarned = totalAmount - principal;

            model.InterestRate = (decimal) interest;
            model.Duration = duration;
            model.TotalAmount = (decimal) totalAmount;
            model.TotalInterest = (decimal) interestEarned;

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
    }
}
