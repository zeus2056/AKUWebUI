using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Student
{
	public class CreateStudentExam
	{

        [Required(ErrorMessage ="Öğrenci Zorunludur...")]
        public int StudentId { get; set; }
        [Required(ErrorMessage ="Kur Zorunludur...")]
        public int RateId { get; set; }
        [Range(1,100,ErrorMessage ="Geçme Notu 1 ile 100 arası olmalıdır")]
        public int  SuccessNote { get; set; }
        [Required(ErrorMessage ="Sınav Tarihi Boş Geçilemez...")]
        public DateTime ExamDate { get; set; }
        [Required(ErrorMessage ="Sınav Dosyası Zorunludur..")]
        public IFormFile ExamPath { get; set; }
    }
}
