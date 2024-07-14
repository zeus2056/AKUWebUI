using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class RateStudent
	{
        public int RateStudentId { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public int RateId { get; set; }
        public double RatePrice { get; set; }
        public bool SuccessState { get; set; }
        public bool GraduationState { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public DateTime StartRateDate { get; set; }
        public DateTime EndRateDate { get; set; }
        public Student Student { get; set; }
        public Exam Exam { get; set; }
        public Rate Rate { get; set; }
        public List<Payment> Payments { get; set; }
        public List<FrontPayment> FrontPayments { get; set; }
        public List<RatePaymentInfo> RatePaymentInfos { get; set; }
        public bool State { get; set; }
    }
}
