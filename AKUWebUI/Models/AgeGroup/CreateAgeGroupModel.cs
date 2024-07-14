using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.AgeGroup
{
	public class CreateAgeGroupModel
	{
		[Required(ErrorMessage ="Name is required...")]
        public string Name { get; set; }
        [Required(ErrorMessage ="StartAge is required...")]
        public int StartAge { get; set; }
		[Required(ErrorMessage = "EndAge is required...")]
		public int EndAge { get; set; }
    }
}
