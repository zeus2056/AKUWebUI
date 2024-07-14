using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Permission
{
	public class UpdatePermissionModel
	{
		[Required(ErrorMessage ="İzinId zorunludur...")]
        public int PermissionId { get; set; }
		[Required(ErrorMessage = "İzin  Adı zorunludur...")]
		public string Name
		{
			get; set;
		}
		[Required(ErrorMessage = "Gün Sayısı boş geçilemez...")]
		[Range(1, 100, ErrorMessage = "Gün Sayısı 1 ile 100 arası olmalıdır...")]
		public int DayCount
		{
			get; set;
		}
		[Required(ErrorMessage = "Yıl Sayısı boş geçilemez...")]
		[Range(1, 100, ErrorMessage = "Yıl Sayısı 1 ile 100 arası olmalıdır...")]
		public int YearCount
		{
			get; set;
		}
	}
}
