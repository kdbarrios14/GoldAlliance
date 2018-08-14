using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldAlliance.Models
{
    public class TdInterestViewModel
    {

        public int Duration { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name ="Interest Rate")]
        public decimal InterestRate { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name ="Total Amount")]
        public decimal TotalAmount { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Display(Name = "Total Interest")]
        public decimal TotalInterest { get; set; }
    }
}