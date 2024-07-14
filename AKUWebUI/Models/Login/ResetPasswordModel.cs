using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Login
{
	public class ResetPasswordModel
	{
        [Required(ErrorMessage ="Id is not null")]
        public int Id { get; set; }
        [Required(ErrorMessage ="Token is not null")]
        public string ConfirmCode { get; set; }
    }
}
