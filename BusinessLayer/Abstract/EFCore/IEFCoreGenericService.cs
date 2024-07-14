using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreGenericService<TEntity> where TEntity : class ,new()
	{
		Task AddAsync(TEntity entity);
		Task<List<TEntity>> GetAllAsync();
		Task<TEntity> GetByIdAsync(int id);
		Task<List<TEntity>> GetAllFilteredAsync(Expression<Func<TEntity, bool>> expression);
		void Update(TEntity entity);
		void Delete(TEntity entity);
	}
}
