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
	public class EFCorePaymentTypeManager : IEFCorePaymentTypeService
	{
		private readonly IEFCorePaymentTypeRepository _paymentTypeRepository;

		public EFCorePaymentTypeManager(IEFCorePaymentTypeRepository paymentTypeRepository)
		{
			_paymentTypeRepository = paymentTypeRepository;
		}

		public async Task AddAsync(PaymentType entity)
		{
			await _paymentTypeRepository.AddAsync(entity);
		}

		public void Delete(PaymentType entity)
		{
			_paymentTypeRepository.Delete(entity);
		}

		public async Task<PaymentType> FindBySlugAsync(string slug)
		{
			return await _paymentTypeRepository.FindBySlugAsync(slug);
		}

		public async Task<List<PaymentType>> GetAllAsync()
		{
			return await _paymentTypeRepository.GetAllAsync();
		}

		public async Task<List<PaymentType>> GetAllFilteredAsync(Expression<Func<PaymentType, bool>> expression)
		{
			return await _paymentTypeRepository.GetAllFilteredAsync(expression);
		}

		public async Task<PaymentType> GetByIdAsync(int id)
		{
			return await _paymentTypeRepository.GetByIdAsync(id);
		}

		public void Update(PaymentType entity)
		{
			_paymentTypeRepository.Update(entity);
		}
	}
}
