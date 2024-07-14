using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class RatePaymentInfo
	{
        public int Id { get; set; }
        public int RateId { get; set; }
        public Rate Rate { get; set; }
        public int RateStudentId { get; set; }
        public RateStudent RateStudent { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
