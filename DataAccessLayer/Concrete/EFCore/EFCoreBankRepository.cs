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
	public class EFCoreBankRepository : EFCoreGenericRepository<Bank> , IEFCoreBankRepository
	{
		public async Task<Bank> FindBySlugAsync(string slug)
		{
			using (var context = new WebContext())
			{
				return await context.Banks.Include(b => b.Payments).FirstOrDefaultAsync(b => b.Slug == slug);
			}
		}
	}
}
