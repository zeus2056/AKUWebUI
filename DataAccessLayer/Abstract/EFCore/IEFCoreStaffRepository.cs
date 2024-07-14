using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreStaffRepository : IEFCoreGenericRepository<AppUser>
	{
		Task<AppUser> FindBySlugAsync(string? slug);
		Task<AppUser> GetAllPermissionByIdAsync(int id);
	}
}
