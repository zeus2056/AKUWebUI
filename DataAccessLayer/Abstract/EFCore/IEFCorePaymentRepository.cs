using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCorePaymentRepository : IEFCoreGenericRepository<Payment>
	{
		Task<List<Payment>> GetAllPaymentsAsync(int branchId);
		Task<List<Payment>> GetAllPaymentByRateStudentAsync(int rateStudentId);
		Task<List<Payment>> FindPaymentsByIdAsync(int studentId);
		Task<Payment> FindByIdsAsync(int rateId, int studentId, int rateStudenId);
	}
}
