using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Bank
{
    public class UpdateBankModel
    {
        [Required(ErrorMessage ="BankaId boş geçilemez...")]
        public int BankId { get; set; }
        [Required(ErrorMessage = "BankaAdı boş geçilemez...")]
        public string BankName { get; set; }
    }
}
