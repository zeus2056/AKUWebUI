using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete.EFCore
{
	public class EFCoreExamManager : IEFCoreExamService
	{
		private readonly IEFCoreExamRepository _examRepository;

		public EFCoreExamManager(IEFCoreExamRepository examRepository)
		{
			_examRepository = examRepository;
		}

		public async Task AddAsync(Exam entity)
		{
			await _examRepository.AddAsync(entity);
		}

		public void Delete(Exam entity)
		{
			_examRepository.Delete(entity);
		}

		public async Task<List<Exam>> FindByBranchIdAsync(int branchId)
		{
			return await _examRepository.FindByBranchIdAsync(branchId);
		}

		public async Task<List<Exam>> GetAllAsync()
		{
			return await _examRepository.GetAllAsync();
		}

		public async Task<List<Exam>> GetAllFilteredAsync(Expression<Func<Exam, bool>> expression)
		{
			return await _examRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<Exam>> GetAllWithIncludesAsync()
		{
			return await _examRepository.GetAllWithIncludesAsync();
		}

		public async Task<Exam> GetByIdAsync(int id)
		{
			return await _examRepository.GetByIdAsync(id);
		}

		public async Task<List<RateStudent>> GetRateStudentsByRateIdAsync(int rateId)
		{
			return await _examRepository.GetRateStudentsByRateIdAsync(rateId);
		}

		public void Update(Exam entity)
		{
			_examRepository.Update(entity);
		}
	}
}
