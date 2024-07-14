using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Filtered
{
	public class GenelFilteredModel
	{
        public int StudentId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
		[Required]
		public DateTime EndDate { get; set; }
    }
}
