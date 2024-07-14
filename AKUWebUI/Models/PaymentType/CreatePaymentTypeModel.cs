using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.PaymentType
{
	public class CreatePaymentTypeModel
	{
        [Required(ErrorMessage = "Ödeme Tipi adı boş geçilemez...")]
        public string PaymentTypeName { get; set; }
    }
}
