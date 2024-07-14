using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreExamRepository : IEFCoreGenericRepository<Exam>
	{
		Task<List<Exam>> GetAllWithIncludesAsync();
		Task<List<RateStudent>> GetRateStudentsByRateIdAsync(int rateId);
		Task<List<Exam>> FindByBranchIdAsync(int branchId);
	}
}
