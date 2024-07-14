using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete.EFCore
{
	public class EFCoreStaffManager : IEFCoreStaffService
	{
		private readonly
			 IEFCoreStaffRepository _staffRepository;

		public EFCoreStaffManager(IEFCoreStaffRepository staffRepository)
		{
			_staffRepository = staffRepository;
		}

		public async Task AddAsync(AppUser entity)
		{
			await _staffRepository.AddAsync(entity);
		}

		public void Delete(AppUser entity)
		{
			_staffRepository.Delete(entity);
		}

        public async Task<AppUser> FindBySlugAsync(string? slug)
        {
            return await _staffRepository.FindBySlugAsync(slug);
        }

        public async Task<List<AppUser>> GetAllAsync()
		{
			return await _staffRepository.GetAllAsync();
		}

		public async Task<AppUser> GetAllById(int id)
		{
			return await _staffRepository.GetAllPermissionByIdAsync(id);
		}

		public async Task<List<AppUser>> GetAllFilteredAsync(Expression<Func<AppUser, bool>> expression)
		{
			return await _staffRepository.GetAllFilteredAsync(expression);
		}

		public async Task<AppUser> GetByIdAsync(int id)
		{
			return await _staffRepository.GetByIdAsync(id);
		}

		public void Update(AppUser entity)
		{
			_staffRepository.Update(entity);
		}
	}
}
