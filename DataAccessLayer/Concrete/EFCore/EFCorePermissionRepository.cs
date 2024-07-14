using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EFCore
{
	public class EFCorePermissionRepository : EFCoreGenericRepository<Permission>, IEFCorePermissionRepository
	{
		public async Task<Permission> FindByNameAsync(string name)
		{
			using (var context = new WebContext())
			{
				return await context.Permissions.FirstOrDefaultAsync(p => p.Name == name);
			}
		}

		public async Task<Permission> FindNextByIdAsync(int id, int yearCount)
		{
			using(var context = new WebContext())
			{
				return await context.Permissions.Where(p => p.PermissionId != id && p.YearCount >
				yearCount).OrderBy(p => p.YearCount).FirstOrDefaultAsync();
			}
		}

		public async Task<List<Permission>> GetAllIncludeAsync()
		{
			using (var context = new WebContext())
			{
				return await context.Permissions.ToListAsync();
			}
		}
	}
}
