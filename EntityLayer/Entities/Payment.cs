using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Payment
	{
        public int PaymentId { get; set; }
        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }
        public int RateStudentId { get; set; }
        public RateStudent RateStudent { get; set; }
        public int StudentId
		{
			get; set;
		}
        public Student Student { get; set; }
        public int? ParentId { get; set; }
        
        public Parent? Parent { get; set; }
        public double LastPaymentPrice { get; set; }
        public DateTime PaymentDate { get; set; }
        public double PaymentPrice { get; set; }
        public double   RemaningPaymentPrice { get; set; }
        public double Bakiye { get; set; }
        public int RateId { get; set; }
        public Rate Rate { get; set; }
        public bool State { get; set; }
        public int? BankId { get; set; }
        public Bank? Bank { get; set; }
    }

}
