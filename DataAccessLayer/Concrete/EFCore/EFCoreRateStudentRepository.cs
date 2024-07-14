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
	public class EFCoreRateStudentRepository : EFCoreGenericRepository<RateStudent>, IEFCoreRateStudentRepository
	{
		public async Task<RateStudent> FindByIdandSlugAsync(int rateId, string studentSlug)
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.Include(r => r.Student).Include(r => r.Rate).ThenInclude(r => r.AgeGroup).FirstOrDefaultAsync(r => r.RateId == rateId && r.Student.Slug == studentSlug);
			}
		}

		public async Task<List<RateStudent>> FindByRateIdAsync(int rateId)
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.Include(r => r.Student).Where(r => r.RateId == rateId).ToListAsync();
			}
		}

		public async Task<List<RateStudent>> GetStudentRatesAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.Include(r => r.Student).Include(r => r.Rate).Where(r => r.StudentId == studentId).ToListAsync();
			}
		}

		public async Task<RateStudent> LastRateStudentByAsync()
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.OrderBy(r => r.RateId).LastOrDefaultAsync();
			}
		}

		public async Task UpdateRange(List<RateStudent> rateStudents)
		{
			using (var context = new WebContext())
			{
				context.RateStudents.UpdateRange(rateStudents);
				await context.SaveChangesAsync();
			}
		}
	}
}
