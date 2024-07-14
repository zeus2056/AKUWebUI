using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreStudentService : IEFCoreGenericService<Student>
	{
		Task<Student> GetUserBySlugAsync(string slug);
		Task<List<Student>> GetAllStudentsWithIncludesAsync(int branchId);
		Task<List<Student>> GetAllWithIncludesAsync();
		Task<List<Student>> FindStudentsByNameAsync(string name);
		Task<Student> FindBySlugAsync(string slug);
		Task<List<Student>> GetAllPaymentsAsync(int studentId);
		Task<List<Student>> GetAllByBranchAndRateId(int ageGroupId, int branchId);
	}
}
