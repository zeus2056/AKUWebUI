using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreFrontPaymetRepository : IEFCoreGenericRepository<FrontPayment>
	{
		Task<List<FrontPayment>> GetAllFrontPaymentsAsync();
		Task<FrontPayment> FindByIdAsync(int rateId, int rateStudentId);
	}
}
