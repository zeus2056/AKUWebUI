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
	public class EFCoreRateManager : IEFCoreRateService
	{
		private readonly IEFCoreRateRepository _rateRepository;

		public EFCoreRateManager(IEFCoreRateRepository rateRepository)
		{
			_rateRepository = rateRepository;
		}

		public async Task AddAsync(Rate entity)
		{
			await _rateRepository.AddAsync(entity);
		}

		public void Delete(Rate entity)
		{
			_rateRepository.Delete(entity);
		}

		public async Task<List<Rate>> FindByAgeandBranchAsync(int ageGroupId, int branchId)
		{
			return await _rateRepository.FindByAgeandBranchAsync(ageGroupId, branchId);
		}

		public async Task<Rate> FindByNameAsync(string name)
		{
			return await _rateRepository.FindByNameAsync(name);
		}

		public async Task<Rate> FindBySlugAsync(string slug)
		{
			return await _rateRepository.FindBySlugAsync(slug);
		}

		public async Task<RateStudent> FindIncludeByIdAsync(int rateId)
		{
			return await _rateRepository.FindIncludeByIdAsync(rateId);
		}

		public async Task<List<Rate>> GetAllAsync()
		{
			return await _rateRepository.GetAllAsync();
		}

		public async Task<List<Rate>> GetAllFilteredAsync(Expression<Func<Rate, bool>> expression)
		{
			return await _rateRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<Rate>> GetAllOpenRatesWithAgeGroupAsync(int branchId)
		{
			return await _rateRepository.GetAllOpenRatesWithAgeGroupAsync(branchId);
		}

		public async Task<List<Rate>> GetAllOpensRateWithAgeGroupAsync(int branchId)
		{
			return await _rateRepository.GetAllOpenRatesWithAgeGroupAsync(branchId);
		}

		public async Task<List<Rate>> GetAllWithAgeGroup(int branchId)
		{
			return await _rateRepository.GetAllWithAgeGroup(branchId);
		}

		public async Task<Rate> GetByIdAsync(int id)
		{
			return await _rateRepository.GetByIdAsync(id);
		}

		public async Task<Rate> GetRateWithRateStudentsAsync(int rateId)
		{
			return await _rateRepository.GetRateWithRateStudentsAsync(rateId);
		}

		public void Update(Rate entity)
		{
			_rateRepository.Update(entity);
		}
	}
}
