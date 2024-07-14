using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCorePermissionRepository : IEFCoreGenericRepository<Permission>
	{
		Task<Permission> FindByNameAsync(string name);
		Task<Permission> FindNextByIdAsync(int id,int yearCount);
		Task<List<Permission>> GetAllIncludeAsync();
	}
}
