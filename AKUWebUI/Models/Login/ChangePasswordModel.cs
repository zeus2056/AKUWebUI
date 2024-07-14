using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Login
{
	public class ChangePasswordModel
	{
		[Required(ErrorMessage ="UserId is required...")]
        public int UserId { get; set; }
        [Required(ErrorMessage ="Old Password is required...")]
        public string OldPassword { get; set; }
		[Required(ErrorMessage = "New Password is required...")]
		public string NewPassword { get; set; }
		[Required(ErrorMessage = "ReNew Password is required...")]
		[Compare(nameof(NewPassword),ErrorMessage ="Passwords have been not same...")]
		public string ReNewPassoword { get; set; }
    }
}
