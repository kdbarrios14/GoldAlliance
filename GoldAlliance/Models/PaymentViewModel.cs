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
        public decimal InterestRate { get; set; }

        public int Term { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal MonthlyPayments { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal TotalInterestPaid { get; set; }
    }
}