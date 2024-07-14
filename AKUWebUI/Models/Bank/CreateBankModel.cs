using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Bank
{
	public class CreateBankModel
	{
		[Required(ErrorMessage ="BankName is required...")]
        public string BankName { get; set; }
    }
}
