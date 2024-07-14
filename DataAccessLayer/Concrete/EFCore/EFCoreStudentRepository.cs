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
	public class EFCoreStudentRepository : EFCoreGenericRepository<Student>, IEFCoreStudentRepository
	{
		public async Task<List<Student>> GetAllStudentsWithIncludesAsync(int branchId)
		{
			using (var context = new WebContext())
			{
				return await context.Students.Include(s => s.Rates).Include(s => s.RateStudents).Include(s => s.Branch).Where(s => s.BranchId == branchId && s.State).ToListAsync();
			}
		}

		public async Task<Student> GetUserBySlugAsync(string slug)
		{
			using (var context = new WebContext())
			{
				return await context.Students.Include(s => s.Branch).FirstOrDefaultAsync(s => s.Slug == slug);
			}
		}
		public async Task<List<Student>> GetAllWithIncludesAsync()
		{
			using (var context = new WebContext())
			{
				return await context.Students.Where(s => s.State).Include(s => s.Branch).ToListAsync();
			}
		}

		public async Task<List<Student>> FindStudentsByNameAsync(string name)
		{
			using (var context = new WebContext())
			{
				return await context.Students.Include(s => s.Branch).Where(s => (s.Name + s.Surname).ToUpper().Contains(name.ToUpper()) && s.State).ToListAsync();
			}
		}

		public async Task<Student> FindBySlugAsync(string slug)
		{
			using (var context = new WebContext())
			{
				return await context.Students.FirstOrDefaultAsync(s => s.Slug == slug);
			}
		}

		public async Task<List<Student>> GetAllPaymentsAsync(int studentId)
		{
			using (var context = new WebContext())
			{
				return await context.Students.Include(r => r.RateStudents).ThenInclude(r => r.Rate).Include(r => r.Payments).Where(s => s.StudentId == studentId).ToListAsync();
			}
		}

		public async Task<List<Student>> GetAllByBranchAndRateId(int ageGroupId, int branchId)
		{
			using (var context = new WebContext())
			{
				var age = context.AgeGroups.AsNoTracking().FirstOrDefault(a => a.AgeGroupId == ageGroupId);
				return await context.Students.Include(s => s.RateStudents).Where(s => s.BranchId == branchId && (s.Age >= age.StartAge && s.Age <= age.EndAge) && s.State).ToListAsync();
			}
		}
	}
}
