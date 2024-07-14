using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreStaffService : IEFCoreGenericService<AppUser>
	{
		Task<AppUser> FindBySlugAsync(string? slug);
		Task<AppUser> GetAllById(int id);
	}
}
