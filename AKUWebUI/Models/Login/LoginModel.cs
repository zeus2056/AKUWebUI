using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Login
{
	public class LoginModel
	{
        [Required(ErrorMessage = "Email is not null")]
        [EmailAddress(ErrorMessage ="Email format is not correct")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is not null")]
        public string Password { get; set; }
    }
}
