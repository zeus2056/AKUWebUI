using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete.EFCore
{
    public class EFCoreGenericRepository<TEntity> : IEFCoreGenericRepository<TEntity> where TEntity : class, new()
    {
        public async Task AddAsync(TEntity entity)
        {
            using (var context = new WebContext())
            {
                await context.Set<TEntity>().AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public void Delete(TEntity entity)
        {
            using (var context = new WebContext())
            {
                context.Set<TEntity>().Remove(entity);
                context.SaveChanges();
            }
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            using (var context = new WebContext())
                return await context.Set<TEntity>().ToListAsync();
        }

        public async Task<List<TEntity>> GetAllFilteredAsync(Expression<Func<TEntity, bool>> expression)
        {
            using (var context = new WebContext())
                return await context.Set<TEntity>().Where(expression).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            using (var context = new WebContext())
                return await context.Set<TEntity>().FindAsync(id);
        }

        public void Update(TEntity entity)
        {
            using (var context = new WebContext())
            {
                context.Set<TEntity>().Update(entity);
                context.SaveChanges();
            }
        }
    }
}
