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
	public class EFCoreBankManager : IEFCoreBankService
	{
		private readonly IEFCoreBankRepository _bankRepository;

		public EFCoreBankManager(IEFCoreBankRepository bankRepository)
		{
			_bankRepository = bankRepository;
		}

		public async Task AddAsync(Bank entity)
		{
			await _bankRepository.AddAsync(entity);	
		}

		public void Delete(Bank entity)
		{
			_bankRepository.Delete(entity);
		}

		public async Task<Bank> FindBySlugAsync(string slug)
		{
			return await _bankRepository.FindBySlugAsync(slug);
		}

		public async Task<List<Bank>> GetAllAsync()
		{
			return await _bankRepository.GetAllAsync();
		}

		public async Task<List<Bank>> GetAllFilteredAsync(Expression<Func<Bank, bool>> expression)
		{
			return await _bankRepository.GetAllFilteredAsync(expression);
		}

		public async Task<Bank> GetByIdAsync(int id)
		{
			return await _bankRepository.GetByIdAsync(id);
		}

		public void Update(Bank entity)
		{
			_bankRepository.Update(entity);
		}
	}
}
