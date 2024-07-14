using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCorePermissionService : IEFCoreGenericService<Permission>
	{

		Task<Permission> FindByNameAsync(string name);
		Task<Permission> FindNextByIdAsync(int id, int yearCount);
		Task<List<Permission>> GetAllIncludeAsync();
	}
}
