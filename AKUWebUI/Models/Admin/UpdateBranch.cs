using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Admin
{
    public class UpdateBranch
    {
        [Required(ErrorMessage ="BranchId is required...")]
        public int BranchId { get; set; }
        [Required(ErrorMessage = "BranchName is required...")]
        public string BranchName { get; set; }
    }
}
