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
	public class EFCoreExamResultRepository : EFCoreGenericRepository<ExamResult> , IEFCoreExamResultRepository
	{
		public async Task AddRange(List<ExamResult> examResults)
		{
			using (var context = new WebContext())
			{
				await context.ExamResults.AddRangeAsync(examResults);
				await context.SaveChangesAsync();
			}
		}

		public async Task<ExamResult> FindByExamIdAsync(int examId)
		{
			using(var context = new WebContext())
			{
				return await context.ExamResults.FirstOrDefaultAsync(e => e.ExamId == examId);
			}
		}

		public async Task<Exam> FindByIdAndSlugAsync(string slug, int id)
		{
			using (var context = new WebContext())
			{
				return await context.Exams.Include(e => e.ExamResult).Include(e => e.Rate).ThenInclude(r => r.RateStudents).ThenInclude(r => r.Student).FirstOrDefaultAsync(e => e.Rate.Slug == slug && e.ExamId == id);
			}
		}

		public async Task<List<ExamResult>> FindsByExamIdAsync(int examId)
		{
			using (var context = new WebContext())
			{
				return await context.ExamResults.Include(e => e.Student).Where(e => e.ExamId == examId).ToListAsync();
			}
		}

		public async Task<List<ExamResult>> GetExamResultsByStudentIdAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return await context.ExamResults.Include(e => e.Exam).Include(e => e.Student).Include(e => e.Rate).Where(e => e.StudentId == studentId).ToListAsync();
			}
		}

		public  void UpdateRange(List<ExamResult> examResults)
		{
			using(var context = new WebContext())
			{
				foreach (var item in examResults)
				{
					context.ExamResults.Update(item);
				}
				 context.SaveChanges();
			}
		}
	}
}
