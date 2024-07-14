using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class StaffPermission
	{
        public int Id { get; set; }
        [ForeignKey(nameof(AppUser))]
        public int StaffId { get; set; }
        public AppUser AppUser { get; set; }
        public int RestPermissionCount { get; set; }
        public DateTime Date { get; set; }
    }
}
