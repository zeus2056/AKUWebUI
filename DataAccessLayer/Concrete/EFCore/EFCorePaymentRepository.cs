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
	public class EFCorePaymentRepository : EFCoreGenericRepository<Payment>, IEFCorePaymentRepository
	{
		public async Task<Payment> FindByIdsAsync(int rateId, int studentId, int rateStudenId)
		{
			using (var context = new WebContext())
			{
				return await context.Payments.Include(p => p.Rate).Include(p => p.RateStudent).Include(p => p.Student)
					.FirstOrDefaultAsync(p => p.StudentId == studentId && p.RateId == rateId && p.RateStudentId == rateStudenId);
				;
			}
		}

		public async Task<List<Payment>> FindPaymentsByIdAsync(int studentId)
		{using (var context = new WebContext())
			{
				return await context.Payments.Include(p => p.Rate).Include(p => p.RateStudent).Include(p => p.Student).Where(p => p.StudentId == studentId).ToListAsync();
;			}
		}

		public async Task<List<Payment>> GetAllPaymentByRateStudentAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return    await(context.Payments.Include(p => p.RateStudent).Include(p => p.Parent).ThenInclude(p => p.Student).Include(p => p.Rate).Where(p => p.StudentId == studentId).ToListAsync());
			}
		}

		public async Task<List<Payment>> GetAllPaymentsAsync(int branchId)
		{
			using (var context = new WebContext())
			{
				return  branchId == 0 ? await context.Payments.Include(p => p.Parent).Include(r => r.RateStudent).Include(p => p.PaymentType).Include(p => p.Rate).Include(r => r.Student).Where(p => p.Bakiye != 0).ToListAsync() : await context.Payments.Include(p => p.Parent).Include(p => p.Student).Include(p => p.Rate).Include(r => r.RateStudent).Include(p => p.PaymentType).Where(p => p.Rate.BranchId == branchId && p.Bakiye  != 0).ToListAsync();		}
		}
	}
}
