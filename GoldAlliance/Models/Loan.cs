//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoldAlliance.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Loan
    {
        public int LoanId { get; set; }
        public int MemberId { get; set; }
        public int LoanTypeId { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public System.DateTime IssueDate { get; set; }

        public decimal Amount { get; set; }

        [Display(Name ="Amount Paid")]
        public Nullable<decimal> PaymentAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name ="Upcoming Payment Date")]
        public System.DateTime PaymentDate { get; set; }

        [Display(Name ="Term(years)")]
        public int Term_Years { get; set; }
    
        public virtual LoanType LoanType { get; set; }
        public virtual Member Member { get; set; }
    }
}
