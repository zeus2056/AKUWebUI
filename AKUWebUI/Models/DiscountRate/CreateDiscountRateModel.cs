using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.DiscountRate
{
	public class CreateDiscountRateModel
	{
        [Required(ErrorMessage ="İndirim Oranı İsmi boş geçilemez...")]
        public string DiscountRateName { get; set; }
		[Required(ErrorMessage = "İndirim Oranı  boş geçilemez...")]
		[Range(1,99,ErrorMessage ="İndirim Oranı 1 ile 99 arası olmalıdır...")]
		public int DiscountRate { get; set; }
    }
}
