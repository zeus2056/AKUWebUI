using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Student
{
	public class UpdateStudentModel
	{
		[Required(ErrorMessage ="StudentId is required...")]
        public int StudentId { get; set; }
        public int BranchId
		{
			get; set;
		} = -1;
		[Required(ErrorMessage = "Name is required...")]
		public string Name
		{
			get; set;
		}
		[Required(ErrorMessage = "Surname is required...")]
		public string Surname
		{
			get; set;
		}
		[Required(ErrorMessage = "TC is required...")]
		[StringLength(11, MinimumLength = 11, ErrorMessage = "TC must be 11 charecter...")]
		public string TC
		{
			get; set;
		}
		[Required(ErrorMessage = "Phone is required...")]
		[StringLength(11, MinimumLength = 11, ErrorMessage = "Phone must be 11 charecter...")]
		public string Phone
		{
			get; set;
		}
		public bool Gender
		{
			get; set;
		}
		[Required(ErrorMessage = "Date is required...")]
		public DateTime Date
		{
			get; set;
		}
		[Required(ErrorMessage = "Okul Adı Zorunludur....")]
		public string SchoolName
		{
			get; set;
		}
		[Required(ErrorMessage = "Sınıf Boş geçilemez...")]
		[Range(1, int.MaxValue, ErrorMessage = "Sınıf 1 den küçük olamaz...")]
		public int Class
		{
			get; set;
		}
        public IFormFile? ImagePath { get; set; }
		public IFormFile? ExamPath
		{
			get;set;
		}
    }
}
