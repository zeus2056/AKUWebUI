using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Rate
{
	public class EditRateModel
	{
        [Required(ErrorMessage = "RateId is required...")]
        public int RateId { get; set; }
        [Required(ErrorMessage = "RateName is required...")]
		public string RateName
		{
			get; set;
		}
		[Required(ErrorMessage = "Date is required...")]
		public double RateDate
		{
			get; set;
		}
		[Required(ErrorMessage = "Price is required...")]
		public double RatePrice
		{
			get; set;
		}
		[Required(ErrorMessage = "RateStartDate is required...")]
		public DateTime RateStartDate
		{
			get; set;
		}
		[Required(ErrorMessage = "AgeGroup is required...")]
		public int AgeGroupId
		{
			get; set;
		}
        [Required(ErrorMessage = "Açıklama Zorunludur...")]
        public string Description { get; set; }
    }
}

