using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCorePaymentService : IEFCoreGenericService<Payment>
	{
		Task<List<Payment>> GetAllPaymentAsync(int branchId);
		Task<List<Payment>> GetAllPaymentByRateStudentAsync(int studentId);
		Task<List<Payment>> FindPaymentsByIdAsync(int studentId);
		Task<Payment> FindByIdsAsync(int rateId, int studentId, int rateStudenId);
	}
}
