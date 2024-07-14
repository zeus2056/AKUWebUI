namespace AKUWebUI.Models.Staff
{
	public class ChangeImageModel
	{
        public int UserId { get; set; }
        public IFormFile ImagePath { get; set; }
    }
}
