using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Register
{
    public class EmailConfirmModel
    {
        [Required(ErrorMessage ="Id is not null")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Id is not null")]
        public string ConfirmCode { get; set; }
    }
}
