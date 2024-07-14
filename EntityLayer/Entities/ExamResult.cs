using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class ExamResult
	{
        public int ExamResultId { get; set; }
        public int StudentId { get; set; }
        public int RateId { get; set; }
        public int ExamId { get; set; }
        public double Note { get; set; }
        public bool isStatus { get; set; } = false;
        public Student Student { get; set; }
        public Rate Rate { get; set; }
        public Exam Exam { get; set; }
    }
}
