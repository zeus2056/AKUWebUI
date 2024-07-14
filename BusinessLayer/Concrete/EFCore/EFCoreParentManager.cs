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
	public class EFCoreParentManager : IEFCoreParentService
	{
		private readonly IEFCoreParentRepository _parentRepository;

		public EFCoreParentManager(IEFCoreParentRepository parentRepository)
		{
			_parentRepository = parentRepository;
		}

		public async Task AddAsync(Parent entity)
		{
			await _parentRepository.AddAsync(entity);
		}

		public void Delete(Parent entity)
		{
			_parentRepository.Delete(entity);
		}

		public async Task<Parent> FindByIdWithStudentAsync(int id)
		{
			return await _parentRepository.FindByIdWithStudentAsync(id);
		}

		public async Task<Parent> FindBySlugAsync(string slug)
		{
			return await _parentRepository.FindBySlugAsync(slug);
		}

		public async Task<List<Parent>> GetAllAsync()
		{
			return await _parentRepository.GetAllAsync();
		}

		public async Task<List<Parent>> GetAllFilteredAsync(Expression<Func<Parent, bool>> expression)
		{
			return await _parentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<Parent> GetByIdAsync(int id)
		{
			return await _parentRepository.GetByIdAsync(id);
		}

		public void Update(Parent entity)
		{
			_parentRepository.Update(entity);
		}
	}
}
