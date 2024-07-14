using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.AgeGroup
{
    public class EditAgeGroupModel
    {
        [Required(ErrorMessage = "Id is required...")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required...")]
        public string Name
        {
            get; set;
        }
        [Required(ErrorMessage ="Başlangıç yaşı zorunludur...")]
        public int StartAge { get; set; }
		[Required(ErrorMessage = "Bitiş yaşı zorunludur...")]
		public int EndAge { get; set; }
    }
}
