using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	[Keyless]
	public class ExamStudent
	{
        [ForeignKey(nameof(Student))]
        public int ExamStudentId { get; set; }
        public Student Student { get; set; }
        [ForeignKey(nameof(Exam))]
        public int StudentExamId { get; set; }
        public Exam Exam { get; set; }
    }
}
