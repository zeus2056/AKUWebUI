using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.OpenRate
{
	public class RemoveStudentModel
	{
        [Required(ErrorMessage ="Id zorunludur...")]
        public int RateId { get; set; }
		[Required(ErrorMessage = "Öğrenci Adı zorunludur...")]
		public int RateStudentId { get; set; }
    }
}
