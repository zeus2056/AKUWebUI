using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.PaymentType
{
	public class UpdatePaymentTypeModel
	{
		[Required(ErrorMessage = "ID boş geçilemez...")]
        public int PaymentTypeId { get; set; }
		[Required(ErrorMessage ="Ödeme Tipi boş geçilemez...")]
        public string PaymentTypeName { get; set; }
    }
}
