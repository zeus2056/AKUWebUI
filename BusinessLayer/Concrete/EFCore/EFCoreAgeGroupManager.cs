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
	public class EFCoreAgeGroupManager : IEFCoreAgeGroupService
	{
		private readonly IEFCoreAgeGroupRepository _ageGroupRepository;

		public EFCoreAgeGroupManager(IEFCoreAgeGroupRepository ageGroupRepository)
		{
			_ageGroupRepository = ageGroupRepository;
		}

		public async Task AddAsync(AgeGroup entity)
		{
			await _ageGroupRepository.AddAsync(entity);
		}

		public void Delete(AgeGroup entity)
		{
			_ageGroupRepository.Delete(entity);
		}

		public async Task<List<AgeGroup>> GetAllAsync()
		{
			return await _ageGroupRepository.GetAllAsync();
		}

		public async Task<List<AgeGroup>> GetAllFilteredAsync(Expression<Func<AgeGroup, bool>> expression)
		{
			return await _ageGroupRepository.GetAllFilteredAsync(expression);
		}

		public async Task<AgeGroup> GetByIdAsync(int id)
		{
			return await _ageGroupRepository.GetByIdAsync(id);
		}

		public void Update(AgeGroup entity)
		{
			_ageGroupRepository.Update(entity);
		}
	}
}
