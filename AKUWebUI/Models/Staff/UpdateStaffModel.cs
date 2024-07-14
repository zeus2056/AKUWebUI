using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Staff
{
	public class UpdateStaffModel
	{
		[Required(ErrorMessage ="StaffId is required...")]
        public int StaffId { get; set; }
        [Required(ErrorMessage = "Name is required...")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required...")]
        public string Surname { get; set; }
        [Required(ErrorMessage ="TC is required...")]
        [StringLength(11,MinimumLength =11,ErrorMessage ="TC is required...")]
        public string TC { get; set; }
        [Required(ErrorMessage ="Date is required...")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage ="Phone is required...")]
        [StringLength(11,MinimumLength = 11,ErrorMessage ="PhoneNumber must be 11 character...")]
        public string Phone { get; set; }
        public string? Email { get; set; }
        public IFormFile? ImagePath { get; set; }
    }
}
