using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreHistoryPaymentRepository : IEFCoreGenericRepository<HistoryPayment>
	{
		Task<List<HistoryPayment>> FindByIdAsync(int studentId);
	}
}
