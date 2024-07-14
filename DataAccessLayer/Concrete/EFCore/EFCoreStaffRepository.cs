using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EFCore
{
    public class EFCoreStaffRepository : EFCoreGenericRepository<AppUser>, IEFCoreStaffRepository
    {
        public async Task<AppUser> FindBySlugAsync(string? slug)
        {
            using (var context = new WebContext())
            {
                return await context.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.Slug == slug);
            }
        }

		public async Task<AppUser> GetAllPermissionByIdAsync(int id)
		{
            using (var context = new WebContext())
            {
                return await context.Users.Include(u => u.Permissions).FirstOrDefaultAsync(u => u.Id == id);
            }
		}
	}
}
