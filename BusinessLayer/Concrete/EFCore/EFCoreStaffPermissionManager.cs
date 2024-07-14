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
	public class EFCoreStaffPermissionManager : IEFCoreStaffPermissionService
	{
		private readonly IEFCoreStaffPermissionRepository _staffPermissionRepository;

		public EFCoreStaffPermissionManager(IEFCoreStaffPermissionRepository staffPermissionRepository)
		{
			_staffPermissionRepository = staffPermissionRepository;
		}

		public async Task AddAsync(StaffPermission entity)
		{
			 await _staffPermissionRepository.AddAsync(entity);
		}

		public void Delete(StaffPermission entity)
		{
			_staffPermissionRepository.Update(entity);
		}

		public async Task<List<StaffPermission>> GetAllAsync()
		{
			return await _staffPermissionRepository.GetAllAsync();
		}

		public async Task<List<StaffPermission>> GetAllFilteredAsync(Expression<Func<StaffPermission, bool>> expression)
		{
			return await _staffPermissionRepository.GetAllFilteredAsync(expression);
		}

		public async Task<StaffPermission> GetByIdAsync(int id)
		{
			return await _staffPermissionRepository.GetByIdAsync(id);
		}

		public void Update(StaffPermission entity)
		{
			_staffPermissionRepository.Update(entity);
		}
	}
}
