using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete.EFCore
{
	public class EFCoreStudentManager : IEFCoreStudentService
	{
		private readonly IEFCoreStudentRepository _studentRepository;

		public EFCoreStudentManager(IEFCoreStudentRepository studentRepository)
		{
			_studentRepository = studentRepository;
		}

		public async Task AddAsync(Student entity)
		{
			await _studentRepository.AddAsync(entity);
		}

		public void Delete(Student entity)
		{
			_studentRepository.Delete(entity);
		}

		public async Task<Student> FindBySlugAsync(string slug)
		{
			return await _studentRepository.FindBySlugAsync(slug);
		}

		public async Task<List<Student>> FindStudentsByNameAsync(string name)
		{
			return await _studentRepository.FindStudentsByNameAsync(name);
		}

		public async Task<List<Student>> GetAllAsync()
		{
			return await _studentRepository.GetAllAsync();
		}

		public async Task<List<Student>> GetAllByBranchAndRateId(int ageGroupId, int branchId)
		{
			return await _studentRepository.GetAllByBranchAndRateId(ageGroupId, branchId);	
		}

		public async Task<List<Student>> GetAllFilteredAsync(Expression<Func<Student, bool>> expression)
		{
			return await _studentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<Student>> GetAllPaymentsAsync(int studentId)
		{
			return await _studentRepository.GetAllPaymentsAsync(studentId);
		}

		public async Task<List<Student>> GetAllStudentsWithIncludesAsync(int branchId)
		{
			return await _studentRepository.GetAllStudentsWithIncludesAsync(branchId);
		}

		public async Task<List<Student>> GetAllWithIncludesAsync()
		{
			return await _studentRepository.GetAllWithIncludesAsync();
		}

		public async Task<Student> GetByIdAsync(int id)
		{
			return await _studentRepository.GetByIdAsync(id);
		}

		public async Task<Student> GetUserBySlugAsync(string slug)
		{
			return await _studentRepository.GetUserBySlugAsync(slug);
		}

		public void Update(Student entity)
		{
			_studentRepository.Update(entity);
				
		}
	}
}
