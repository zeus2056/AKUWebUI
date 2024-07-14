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
	public class EFCoreHistoryPaymentRepository : EFCoreGenericRepository<HistoryPayment>, IEFCoreHistoryPaymentRepository
	{
		public async Task<List<HistoryPayment>> FindByIdAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return await context.HistoryPayments.Include(h => h.Rate).Include(h => h.Bank).Include(r => r.PaymentType).Include(r => r.Student).Where(h => h.StudentId == studentId).ToListAsync()
; ;			}
		}
	}
}
