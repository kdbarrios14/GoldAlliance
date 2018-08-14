using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldAlliance.Models
{
    public class TdInterestViewModel
    {
        public int duration { get; set; }
        public decimal interestRate { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal TotalAmount { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        public decimal TotalInterest { get; set; }
    }
}