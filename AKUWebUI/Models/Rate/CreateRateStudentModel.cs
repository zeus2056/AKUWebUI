using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Rate
{
	public class CreateRateStudentModel
	{
		[Required(ErrorMessage ="StudentId is required...")]
		public int StudentId
		{
			get; set;
		}
		[Required(ErrorMessage ="RateId is required...")]
		public int RateId
		{
			get; set;
		}
        public int? DiscountId { get; set; }
        public bool SuccessState
		{
			get; set;
		} = false;
		public bool GraduationState
		{
			get; set;
		} = false;
		[Required(ErrorMessage = "Başlangıç Tarihi Zorunludur...")]
		public DateTime StartRateDate
		{
			get; set;
		}
	}
}
