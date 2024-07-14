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
    public class EFCoreDiscountRateManager : IEFCoreDiscountRateService
    {
        private readonly IEFCoreDiscountRateRepository _discountRateRepository;

        public EFCoreDiscountRateManager(IEFCoreDiscountRateRepository discountRateRepository)
        {
            _discountRateRepository = discountRateRepository;
        }

        public async Task AddAsync(DiscountRate entity)
        {
           await _discountRateRepository.AddAsync(entity);
        }

        public void Delete(DiscountRate entity)
        {
            _discountRateRepository.Delete(entity);
        }

        public async Task<List<DiscountRate>> GetAllAsync()
        {
            return await _discountRateRepository.GetAllAsync();
        }

        public async Task<List<DiscountRate>> GetAllFilteredAsync(Expression<Func<DiscountRate, bool>> expression)
        {
            return await _discountRateRepository.GetAllFilteredAsync(expression);
        }

        public async Task<DiscountRate> GetByIdAsync(int id)
        {
            return await _discountRateRepository.GetByIdAsync(id);
        }

        public void Update(DiscountRate entity)
        {
            _discountRateRepository.Update(entity);
        }
    }
}
