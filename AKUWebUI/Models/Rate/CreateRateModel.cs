using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Rate
{
    public class CreateRateModel
    {
        [Required(ErrorMessage ="RateName is required...")]
        public string RateName
        {
            get; set;
        }
        public int? BranchId { get; set; }
        [Required(ErrorMessage ="Date is required...")]
        public double RateDate
        {
            get; set;
        }
        [Required(ErrorMessage ="Price is required...")]
        public double RatePrice
        {
            get; set;
        }
        [Required(ErrorMessage ="RateStartDate is required...")]
        public DateTime RateStartDate
        {
            get; set;
        }
        [Required(ErrorMessage ="AgeGroup is required...")]
        public int AgeGroupId
        {
            get; set;
        }
        [Required(ErrorMessage = "Açıklama Boş Geçilemez...")]
        [MaxLength(300,ErrorMessage ="En fazla 300 karakter olabilir...")]
        public string Description { get; set; }
    }
}
