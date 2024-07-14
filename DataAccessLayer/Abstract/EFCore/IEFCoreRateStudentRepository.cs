using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreRateStudentRepository : IEFCoreGenericRepository<RateStudent>
	{
		Task<List<RateStudent>> FindByRateIdAsync(int rateId);
		Task<RateStudent> FindByIdandSlugAsync(int rateId, string studentSlug);
		Task UpdateRange(List<RateStudent> rateStudents);
		Task<List<RateStudent>> GetStudentRatesAsync(int studentId);
		Task<RateStudent> LastRateStudentByAsync();
	}
}
