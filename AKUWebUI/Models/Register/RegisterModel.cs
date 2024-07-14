
using EntityLayer;
using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Register
{
	public class RegisterModel
	{
        [Required(ErrorMessage ="Name is not null")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is not null")]
        public string Surname { get; set; }
        [Required(ErrorMessage ="TC is required...")]
        [StringLength(11,MinimumLength =11,ErrorMessage ="TC must be 11 character")]
        public string TC
        {
            get; set;
        }
        [Required(ErrorMessage = "Date is required...")]
        public DateTime Date
        {
            get; set;
        }
        [Required(ErrorMessage = "Gender is required")]
        public bool Gender { get; set; }
        [Required(ErrorMessage = "Username is not null")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email is not null")]
        [EmailAddress(ErrorMessage ="Email format doesn't correct")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is not null")]
        [StringLength(11,MinimumLength =11,ErrorMessage ="Phone must be 11 character")]
        public string Phone { get; set; }
        [Required(ErrorMessage ="Password is not null")]
        public string Password { get; set; }
        [Required(ErrorMessage ="Branch is required")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Role is required")]
        public Rol Role { get; set; } = Rol.Teacher;
        public IFormFile? ImagePath { get; set; }
    }
}
