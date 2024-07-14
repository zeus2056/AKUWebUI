using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreRatePaymentInfoRepository : IEFCoreGenericRepository<RatePaymentInfo>
	{
		Task<List<RatePaymentInfo>> FindByIdsAsync(int studentId);
	}
}
