using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.ExamType
{
	public class UpdateExamTypeModel
	{
        [Required(ErrorMessage ="ExamTypeId is required...")]
        public int ExamTypeId { get; set; }
		[Required(ErrorMessage = "ExamTypeName is required...")]
		public string ExamTypeName { get; set; }
    }
}
