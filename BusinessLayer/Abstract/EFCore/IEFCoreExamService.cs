using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreExamService : IEFCoreGenericService<Exam>
	{
		Task<List<Exam>> GetAllWithIncludesAsync();
		Task<List<Exam>> FindByBranchIdAsync(int branchId);
		Task<List<RateStudent>> GetRateStudentsByRateIdAsync(int rateId);
	}
}
