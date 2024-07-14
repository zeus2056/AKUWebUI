using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Permission
	{
        public int PermissionId { get; set; }
        public string Name { get; set; }
        public int DayCount { get; set; }
        public int YearCount { get; set; }
    }
}
