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
	public class EFCorePermissionManager : IEFCorePermissionService
	{
		private readonly IEFCorePermissionRepository _permissionRepository;

		public EFCorePermissionManager(IEFCorePermissionRepository permissionRepository)
		{
			_permissionRepository = permissionRepository;
		}

		public async Task AddAsync(Permission entity)
		{
			await _permissionRepository.AddAsync(entity);
		}

		public void Delete(Permission entity)
		{
			_permissionRepository.Delete(entity);
		}

		public async Task<Permission> FindByNameAsync(string name)
		{
			return await _permissionRepository.FindByNameAsync(name);
		}

		public async Task<Permission> FindNextByIdAsync(int id, int yearCount)
		{
			return await _permissionRepository.FindNextByIdAsync(id, yearCount);
		}

		public async Task<List<Permission>> GetAllAsync()
		{
			return await _permissionRepository.GetAllAsync();
		}

		public async Task<List<Permission>> GetAllFilteredAsync(Expression<Func<Permission, bool>> expression)
		{
			return await _permissionRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<Permission>> GetAllIncludeAsync()
		{
			return await _permissionRepository.GetAllIncludeAsync();
		}

		public async Task<Permission> GetByIdAsync(int id)
		{
			return await _permissionRepository.GetByIdAsync(id);
		}

		public void Update(Permission entity)
		{
			_permissionRepository.Update(entity);
		}
	}
}
