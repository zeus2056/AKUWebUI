using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Exam
	{
        public int ExamId { get; set; }
        public int RateId { get; set; }
        public int RateStudentId { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public double SuccessNote { get; set; }
        public string ExamPath { get; set; }
        public DateTime ExamDate { get; set; }
        public Student Student { get; set; }
        public RateStudent RateStudent { get; set; }
        public Rate Rate { get; set; }
        public ExamResult ExamResult
        {
            get; set;
        }
    }
}
