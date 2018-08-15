using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldAlliance.Models
{
    public class AccountInterestViewModel
    {
        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name = "Interest on Checking")]
        public decimal AccountInterest { get; set; }
    }
}