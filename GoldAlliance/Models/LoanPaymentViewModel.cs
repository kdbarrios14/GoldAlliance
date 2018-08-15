using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GoldAlliance.Models
{
    public class LoanPaymentViewModel
    {
        [Display(Name = "Amount")]
        [Range(1, int.MaxValue, ErrorMessage = "Only positive number allowed")]
        public decimal PaymentMade { get; set; }
    }
}