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
	public class EFCoreRatePaymentInfoRepository : EFCoreGenericRepository<RatePaymentInfo>,
		IEFCoreRatePaymentInfoRepository
	{
		public async Task<List<RatePaymentInfo>> FindByIdsAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return await context.RatePaymentInfos.Include(r => r.Rate).Include(r => r.Student).Include(r => r.RateStudent).Where(r => r.StudentId == studentId).ToListAsync();
			}
		}
	}
}
