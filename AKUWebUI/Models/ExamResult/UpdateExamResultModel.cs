using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.ExamResult
{
	public class UpdateExamResultModel
	{
		[Required(ErrorMessage = "ExamId zorunludur...")]
		public int ExamId
		{
			get; set;
		}
		[Required(ErrorMessage = "Slug zorunludur")]
		public string Slug
		{
			get; set;
		}
		[Required(ErrorMessage = "Not boş geçilemez...")]
		public List<double> Note { get; set; } = new();
	}
}
