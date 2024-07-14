using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EFCore
{
	public class EFCoreRateRepository : EFCoreGenericRepository<Rate>, IEFCoreRateRepository
	{
		public async Task<Rate> FindBySlugAsync(string slug)
		{
			using (var context = new WebContext())
				return await context.Rates.Include(r => r.AgeGroup).Include(r => r.RateStudents).ThenInclude(r => r.Student).FirstOrDefaultAsync(r => r.Slug == slug);
		}
		public async Task<Rate> FindByNameAsync(string name)
		{
			using (var context = new WebContext())
				return await context.Rates.FirstOrDefaultAsync(r => r.RateName.ToUpper() == name.ToUpper());
		}
		public async Task<Rate> GetRateWithRateStudentsAsync(int rateId)
		{
			using (var context = new WebContext())
			{
				return await context.Rates.Include(r => r.RateStudents).FirstOrDefaultAsync(r => r.RateId == rateId);
			}
		}

		public async Task<List<Rate>> GetAllWithAgeGroup(int branchId)
		{
			using (var context = new WebContext())
			{
				return branchId == 0 ? await context.Rates.Include(r => r.AgeGroup).Include(r => r.Branch).Where(r => r.RateStartDate > DateTime.Now).ToListAsync() :  await context.Rates.Include(r => r.AgeGroup).Include(r => r.Branch).Where(r => r.BranchId == branchId && r.RateStartDate > DateTime.Now).ToListAsync();
			}
		}

		public async Task<List<Rate>> GetAllOpenRatesWithAgeGroupAsync(int branchId)
		{
			using (var context = new WebContext())
			{
				return branchId == 0 ? await context.Rates.Include(r => r.AgeGroup).Include(r => r.RateStudents).Include(r => r.Branch).Where(r => r.RateStartDate < DateTime.Now).ToListAsync() : await context.Rates.Include(r => r.AgeGroup).Include(r => r.Branch).Include(r => r.RateStudents).Where(r => r.BranchId == branchId && r.RateStartDate < DateTime.Now).ToListAsync();
			}
		}

		public async Task<List<Rate>> GetAllOpensRateWithAgeGroupAsync(int branchId)
		{
			using (var context = new WebContext())
			{
				return branchId == 0 ? await context.Rates.Include(r => r.AgeGroup).Include(r => r.Branch).Where(r => r.RateStartDate < DateTime.Now).ToListAsync() : await context.Rates.Include(r => r.AgeGroup).Include(r => r.Branch).Where(r => r.BranchId == branchId && r.RateStartDate < DateTime.Now).ToListAsync();
			}
		}

		public async Task<List<Rate>> FindByAgeandBranchAsync(int ageGroupId, int branchId)
		{
			using (var context = new WebContext())
			{
				return await context.Rates.Include(r => r.RateStudents).ThenInclude(r => r.Student).Include(r => r.AgeGroup).Include(r => r.Branch).Where(r => r.AgeGroupId == ageGroupId && r.BranchId == branchId).ToListAsync();
			}
		}

		public async Task<RateStudent> FindIncludeByIdAsync(int rateStudentId)
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.Include(r => r.Student).FirstOrDefaultAsync(r => r.RateStudentId == rateStudentId);
			}
		}
	}
}
