using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EduVerse.Models.Allclasses
{
    public class PaymentModel
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public decimal Amount { get; set; }
        public string RazorpayPaymentId { get; set; }
        public string RazorpayOrderId { get; set; }
        public string RazorpaySignature { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
    }
}