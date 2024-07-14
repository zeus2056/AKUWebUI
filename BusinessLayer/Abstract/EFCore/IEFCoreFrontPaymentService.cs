using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreFrontPaymentService : IEFCoreGenericService<FrontPayment>
	{
		Task<List<FrontPayment>> GetAllFrontPaymentsAsync();
		Task<FrontPayment> FindByIdAsync(int rateId, int rateStudentId);

	}
}
