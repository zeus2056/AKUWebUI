using System.ComponentModel.DataAnnotations;

namespace AKUWebUI.Models.Permission
{
    public class CreatePermissionModel
    {
        [Required(ErrorMessage ="Name boş geçilemez...")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Gün Sayısı boş geçilemez...")]
        [Range(1,100,ErrorMessage ="Gün Sayısı 1 ile 100 arası olmalıdır...")]
        public int DayCount { get; set; }
        [Required(ErrorMessage = "Yıl Sayısı boş geçilemez...")]
        [Range(1, 100, ErrorMessage = "Yıl Sayısı 1 ile 100 arası olmalıdır...")]
        public int YearCount { get; set; }
    }
}
