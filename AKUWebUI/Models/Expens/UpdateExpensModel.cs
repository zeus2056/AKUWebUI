using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Expens
{
	public class UpdateExpensModel
	{
        public int ExpensId { get; set; }
		[StringLength(300, MinimumLength = 5, ErrorMessage = "Açıklama en az 5 en fazla 300 karakter olmalıdır...")]
		[Required(ErrorMessage ="Açıklama zorunludur...")]
		public string Description
		{
			get; set;
		}
		[Range(0, double.MaxValue, ErrorMessage = "Fiyat 0 den büyük olmalıdır...")]
		[Required(ErrorMessage ="Fiyat Zorunludur...")]
		public double Price
		{
			get; set;
		}
		public int? BranchId
		{
			get; set;
		}
		[Required(ErrorMessage = "Tarih Zorunludur...")]
		public DateTime ExpensDate
		{
			get; set;
		}
	}
}
