using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreRateRepository : IEFCoreGenericRepository<Rate>
	{
		Task<List<Rate>> GetAllWithAgeGroup(int branchId);
		Task<List<Rate>> GetAllOpensRateWithAgeGroupAsync(int branchId);
		Task<List<Rate>> FindByAgeandBranchAsync(int ageGroupId, int branchId);
		Task<Rate> FindBySlugAsync(string slug);
		Task<Rate> GetRateWithRateStudentsAsync(int rateId);
		Task<Rate> FindByNameAsync(string name);
		Task<List<Rate>> GetAllOpenRatesWithAgeGroupAsync(int branchId);
		Task<RateStudent> FindIncludeByIdAsync(int rateId);
	}
}
