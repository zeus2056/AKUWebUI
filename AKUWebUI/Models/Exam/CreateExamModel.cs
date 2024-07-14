using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Exam
{
	public class CreateExamModel
	{
		[Required(ErrorMessage ="Rate is required...")]
        public int RateId { get; set; }
		[Required(ErrorMessage ="ExamType is required...")]
        public int ExamTypeId { get; set; }
		[Required(ErrorMessage ="SuccessNote is required...")]
		[Range(0,100,ErrorMessage ="SuccessNote must be between 0 and 100")]
        public double SuccessNote { get; set; }
		[Required(ErrorMessage ="Sınav Zaman bilgisi zorunludur...")]
        public DateTime Date { get; set; }
		[Required(ErrorMessage ="Sınav Dosyası Zorunludur...")]
        public IFormFile ExamPath { get; set; }
		public bool NormalExam { get; set; } = true;

    }
}
