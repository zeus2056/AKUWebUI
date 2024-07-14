using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models
{
	public class CreatePaymentModel
	{
        public int PaymentId { get; set; }
        [Required(ErrorMessage ="ÖğrenciId zorunlu alandır...")]
        public int StudentId { get; set; }
        [Required(ErrorMessage = "Banka zorunludur...")]
        public int BankId { get; set; }
        public int RateId { get; set; }
        public int RateStudentId { get; set; }
        public int PaymentTypeId { get; set; }
        public int? ParentId { get; set; }
        public double Price { get; set; }
        public double RatePrice { get; set; }
    }
}
