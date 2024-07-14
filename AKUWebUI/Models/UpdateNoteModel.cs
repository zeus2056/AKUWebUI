using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models
{
	public class UpdateNoteModel
	{
        public int ExamId { get; set; }
        [Range(1,100,ErrorMessage ="Not 1 ile 100 arası olmalıdır...")]
        public double Note { get; set; }
    }
}
