using DataAccessLayer.Abstract.EFCore;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EFCore
{
	public class EFCoreStaffPermissionRepository : EFCoreGenericRepository<StaffPermission>
		,IEFCoreStaffPermissionRepository
	{
	}
}
