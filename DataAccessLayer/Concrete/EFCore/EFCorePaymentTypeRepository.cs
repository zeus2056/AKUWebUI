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
	public class EFCorePaymentTypeRepository : EFCoreGenericRepository<PaymentType> , IEFCorePaymentTypeRepository
	{
		public async Task<PaymentType> FindBySlugAsync(string slug)
		{
			using (var context = new WebContext())
			{
				return await context.PaymentTypes.Include(p => p.Payments).FirstOrDefaultAsync(p => p.Slug == slug);
			}
		}
	}
}
