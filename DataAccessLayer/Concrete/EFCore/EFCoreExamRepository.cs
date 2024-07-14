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
	public class EFCoreExamRepository : EFCoreGenericRepository<Exam>, IEFCoreExamRepository
	{
		public async Task<List<Exam>> FindByBranchIdAsync(int branchId)
		{
			using (var context = new WebContext())
			{
				return await (branchId != 0 ? context.Exams.Include(e => e.ExamResult).Include(e => e.Rate).ThenInclude(r => r.AgeGroup).Include(e => e.Student).Where(e => e.Rate.BranchId == branchId).ToListAsync()
					: context.Exams.Include(e => e.ExamResult).Include(e => e.Rate).ThenInclude(r => r.AgeGroup).Include(e => e.Student).ToListAsync());
			}
		}

		public async Task<List<Exam>> GetAllWithIncludesAsync()
		{
			using (var context = new WebContext())
			{
				return await context.Exams.Include(e => e.ExamResult).Include(e => e.Rate).ThenInclude(r => r.AgeGroup).Include(e => e.Student).ToListAsync();
			}
		}

		public async Task<List<RateStudent>> GetRateStudentsByRateIdAsync(int rateId)
		{
			using (var context = new WebContext())
			{
				return await context.RateStudents.Where(r => r.RateId ==  rateId && !r.State).ToListAsync();
			}
		}
	}
}
