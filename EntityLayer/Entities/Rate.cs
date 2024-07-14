using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Rate
	{
        public int RateId { get; set; }
        public int BranchId { get; set; }
        public string RateName { get; set; }
        public double RateDate { get; set; }
        public double RatePrice { get; set; }
        public DateTime RateStartDate { get; set; }
        public bool RateState { get; set; }
        public string Description { get; set; }
        public int AgeGroupId { get; set; }
		public string Slug
		{
			get; set;
		}
		public AgeGroup AgeGroup { get; set; }
        public Branch Branch { get; set; }
        public List<Exam> Exam { get; set; } = new();
        public List<RateStudent> RateStudents { get; set; } = new();
        public List<ExamResult> ExamResults { get; set; }

    }
}
