using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class HistoryPayment
	{
        public int ID { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int RateId { get; set; }
        public Rate Rate { get; set; }
        public int? BankId { get; set; }
        public Bank? Bank { get; set; }
        public DateTime DatePrice { get; set; }
        public double Price { get; set; }
        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }
        public int InstalmentCount { get; set; }
    }
}
