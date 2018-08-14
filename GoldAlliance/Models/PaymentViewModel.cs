using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldAlliance.Models
{
    public class PaymentViewModel
    {
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name ="Interest Rate")]
        public decimal InterestRate { get; set; }

        public int Term { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name ="Monthly Payments")]
        public decimal MonthlyPayments { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name ="Total Interest Paid")]
        public decimal TotalInterestPaid { get; set; }
    }
}