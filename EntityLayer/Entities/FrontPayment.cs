using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class FrontPayment
	{
        public int FrontPaymentId { get; set; }
        public int RateId { get; set; }
        public Rate Rate { get; set; }
        public int RateStudentId { get; set; }
        public RateStudent RateStudent { get; set; }
        public double RemaningPrice { get; set; }
        public double RatePrice { get; set; }
        public double Bakiye { get; set; }
        public DateTime? LastDate { get; set; }
        public bool State { get; set; }

    }
}
