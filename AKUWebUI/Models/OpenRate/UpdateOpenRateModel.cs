using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.OpenRate
{
	public class UpdateOpenRateModel
	{
		[Required(ErrorMessage = "RateId is required...")]
		public int RateId
		{
			get; set;
		}
        [Required(ErrorMessage = "Açıklama Zorunludur...")]
        public string Description { get; set; }
        [Required(ErrorMessage = "RateName is required...")]
		public string RateName
		{
			get; set;
		}
        [Required(ErrorMessage = "Kur Süresi Zorunludur")]
        [Range(1, 9999, ErrorMessage = "Kur Süresi 1 den büyük olmalıdır...")]
        public double RateDate { get; set; }

        public int? BranchId { get; set; }
        [Required(ErrorMessage = "Price is required...")]
		public double RatePrice
		{
			get; set;
		}
		[Required(ErrorMessage = "AgeGroup is required...")]
		public int AgeGroupId
		{
			get; set;
		}
	}
}
