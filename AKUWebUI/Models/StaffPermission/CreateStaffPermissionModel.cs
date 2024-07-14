using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.StaffPermission
{
	public class CreateStaffPermissionModel
	{
		[Required(ErrorMessage = "Gün Sayısı Zorunludur...")]
		public int DayCount { get; set; }
		[Required(ErrorMessage = "PersonelId Zorunludur...")]
		public int UserId { get; set; }
		[Required(ErrorMessage = "Tarih Zorunludur...")]
        public DateTime Date { get; set; }
        public int PermissionId { get; set; }
        public int? OwnPermissionCount { get; set; }
    }
}
