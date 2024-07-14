using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Student
	{
        public int StudentId { get; set; }
        [ForeignKey(nameof(Branch))]
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TC { get; set; }
        public int Age { get; set; }
        public bool State { get; set; } = true;
        public string Phone { get; set; }
        public bool Gender { get; set; }
        public string SchoolName { get; set; }
        public string? ImagePath { get; set; }
        public int Class { get; set; }
        public string? ExamPath { get; set; }
        public string Slug { get; set; }
        public DateTime Date { get; set; }
        public Branch Branch { get; set; }
        public List<Parent> Parents { get; set; } = new();
        public List<Rate> Rates { get; set; } = new();
		public List<RateStudent> RateStudents { get; set; } = new();
		public List<Exam>  Exams { get; set; } = new();
		public List<ExamResult> ExamResults { get; set; } = new();
		public List<Payment> Payments { get; set; } = new();
	}
}
