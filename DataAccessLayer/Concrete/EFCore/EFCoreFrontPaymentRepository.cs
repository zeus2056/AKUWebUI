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
	public class EFCoreFrontPaymentRepository : EFCoreGenericRepository<FrontPayment>, IEFCoreFrontPaymetRepository
	{
		public async Task<List<FrontPayment>> GetAllFrontPaymentsAsync()
		{
			using (var context = new WebContext())
			{
				return await context.FrontPayments.Include(f => f.Rate).Include(r => r.RateStudent).ThenInclude(r => r.Student).Where(r => r.Bakiye == r.RatePrice).ToListAsync();
			}
		}
		public async Task<FrontPayment> FindByIdAsync(int rateId, int rateStudentId)
		{
			using (var context = new WebContext())
			{
				return await context.FrontPayments.Include(f => f.Rate).Include(r => r.RateStudent).ThenInclude(r => r.Student).Where(r => r.RemaningPrice == r.RatePrice).FirstOrDefaultAsync(r => r.RateId == rateId && r.RateStudentId == rateStudentId);
			}
		}
	}
}
