using EntityLayer;
using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Parent
{
	public class CreateParentModel
	{
		[Required(ErrorMessage ="TC is required...")]
		[StringLength(11,MinimumLength = 11,ErrorMessage ="TC must be 11 character...")]
		public string TC
		{
			get; set;
		}
		[Required(ErrorMessage = "Name is required...")]
		public string Name
		{
			get; set;
		}
		[Required(ErrorMessage = "Surname is required...")]
		public string Surname
		{
			get; set;
		}
		[Required(ErrorMessage ="ParentType is required...")]
		public ParentTypes ParentType
		{
			get; set;
		}
		[Required(ErrorMessage = "Job is required...")]
		public string Job
		{
			get; set;
		}
		[Required(ErrorMessage = "PhoneNumber is required...")]
		public string PhoneNumber
		{
			get; set;
		}
		[Required(ErrorMessage = "Mail is required...")]
		[EmailAddress(ErrorMessage ="Mail Format doesn't correct...")]
		public string Mail
		{
			get; set;
		}
		[Required(ErrorMessage = "StudentId is required...")]
		public int StudentId
		{
			get; set;
		}
	}
}
