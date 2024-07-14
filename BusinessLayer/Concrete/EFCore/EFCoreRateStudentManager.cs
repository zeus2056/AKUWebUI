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
	public class EFCoreRateStudentManager : IEFCoreRateStudentService
	{
		private readonly IEFCoreRateStudentRepository _rateStudentRepository;

		public EFCoreRateStudentManager(IEFCoreRateStudentRepository rateStudentRepository)
		{
			_rateStudentRepository = rateStudentRepository;
		}

		public async Task AddAsync(RateStudent entity)
		{
			 await _rateStudentRepository.AddAsync(entity);
		}

		public void Delete(RateStudent entity)
		{
			_rateStudentRepository.Delete(entity);
		}

		public async Task<RateStudent> FindByIdandSlugAsync(int rateId, string studentSlug)
		{
			return await _rateStudentRepository.FindByIdandSlugAsync(rateId, studentSlug);
		}

		public async Task<List<RateStudent>> FindByRateIdAsync(int rateId)
		{
			return await _rateStudentRepository.FindByRateIdAsync(rateId);
		}

		public async  Task<List<RateStudent>> GetAllAsync()
		{
			return await _rateStudentRepository.GetAllAsync();
		}

		public async Task<List<RateStudent>> GetAllFilteredAsync(Expression<Func<RateStudent, bool>> expression)
		{
			return await _rateStudentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<RateStudent> GetByIdAsync(int id)
		{
			return await _rateStudentRepository.GetByIdAsync(id);
		}

		public async Task<List<RateStudent>> GetStudentRatesAsync(int studentId)
		{
			return await _rateStudentRepository.GetStudentRatesAsync(studentId);
		}

		public async Task<RateStudent> LastRateStudentByAsync()
		{
			return await _rateStudentRepository.LastRateStudentByAsync();
		}

		public void Update(RateStudent entity)
		{
			_rateStudentRepository.Update(entity);
		}

		public async Task UpdateRange(List<RateStudent> rateStudents)
		{
			await _rateStudentRepository.UpdateRange(rateStudents);
		}
	}
}
