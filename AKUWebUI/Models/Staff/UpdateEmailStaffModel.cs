using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Staff
{
	public class UpdateEmailStaffModel
	{
		[Required(ErrorMessage ="StaffId is required...")]
        public int StaffId { get; set; }
        [Required(ErrorMessage = "EmailAdress is required...")]
        [EmailAddress(ErrorMessage ="EmailAdress format doesn't correct...")]
        public string Email { get; set; }
    }
}
