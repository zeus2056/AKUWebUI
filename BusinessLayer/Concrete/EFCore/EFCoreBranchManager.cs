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
	public class EFCoreBranchManager : IEFCoreBranchService
	{
		private readonly IEFCoreBranchRepository _branchRepository;

		public EFCoreBranchManager(IEFCoreBranchRepository branchRepository)
		{
			_branchRepository = branchRepository;
		}

		public async Task AddAsync(Branch entity)
		{
			await _branchRepository.AddAsync(entity);
		}

		public void Delete(Branch entity)
		{
			_branchRepository.Delete(entity);
		}

		public async Task<List<Branch>> GetAllAsync()
		{
			return await _branchRepository.GetAllAsync();
		}

		public async Task<List<Branch>> GetAllFilteredAsync(Expression<Func<Branch, bool>> expression)
		{
			return await _branchRepository.GetAllFilteredAsync(expression);
		}

		public async Task<Branch> GetByIdAsync(int id)
		{
			return await _branchRepository.GetByIdAsync(id);
		}

		public void Update(Branch entity)
		{
			_branchRepository.Update(entity);
		}
	}
}
