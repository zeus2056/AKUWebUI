using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Admin
{
    public class CreateBranch
    {
        [Required(ErrorMessage ="BranchName is required")]
        public string BranchName { get; set; }
    }
}
