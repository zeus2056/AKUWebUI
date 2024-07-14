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
	public class EFCoreParentRepository : EFCoreGenericRepository<Parent>, IEFCoreParentRepository
	{
		public async Task<Parent> FindByIdWithStudentAsync(int id)
		{
			using (var context = new WebContext())
				return await context.Parents.Include(p => p.Student).FirstOrDefaultAsync(p => p.ParentId == id);
		}

		public async Task<Parent> FindBySlugAsync(string slug)
		{
			using (var context = new WebContext())
				return await context.Parents.Include(p => p.Student).FirstOrDefaultAsync(p => p.Slug == slug);
		}
	}
}
