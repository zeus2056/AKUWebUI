using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.ChangeRate
{
	public class UpdateChangeRateModel
	{
		[Required(ErrorMessage ="ÖğrenciId zorunludur...")]
        public int RateStudentId { get; set; }
		[Required(ErrorMessage ="KurId zorunludur...")]
        public int RateId { get; set; }
        [Required(ErrorMessage = "YaşGrubuId zorunludur...")]
        public int AgeGroupId { get; set; }
        [Required(ErrorMessage = "ŞubeId zorunludur...")]
        public int BranchId { get; set; }
        public int DiscountId { get; set; }
        public DateTime StartRateDate { get; set; }
    }
}
