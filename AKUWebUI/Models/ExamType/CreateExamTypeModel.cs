using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.ExamType
{
	public class CreateExamTypeModel
	{
		[Required(ErrorMessage ="ExamType is required...")]
        public string ExamTypeName { get; set; }
    }
}
