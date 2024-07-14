using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models
{
	public class EntryNoteModel
	{
		[Required(ErrorMessage ="Sınav Zorunludur...")]
        public int ExamId { get; set; }
		[Range(1,100,ErrorMessage ="Sınav Notu 1 ile 100 arası olmalıdır...")]
        public int Note { get; set; }
    }
}
