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
	public class EFCoreFrontPaymentManager : IEFCoreFrontPaymentService
	{
		private readonly IEFCoreFrontPaymetRepository _frontPaymentRepository;

		public EFCoreFrontPaymentManager(IEFCoreFrontPaymetRepository frontPaymentRepository)
		{
			_frontPaymentRepository = frontPaymentRepository;
		}

		public async Task AddAsync(FrontPayment entity)
		{
			await _frontPaymentRepository.AddAsync(entity);
		}

		public void Delete(FrontPayment entity)
		{
			_frontPaymentRepository.Delete(entity);
		}

		public async Task<FrontPayment> FindByIdAsync(int rateId, int rateStudentId)
		{
			return await _frontPaymentRepository.FindByIdAsync(rateId, rateStudentId);
		}

		public async Task<List<FrontPayment>> GetAllAsync()
		{
			return await _frontPaymentRepository.GetAllAsync();
		}

		public async Task<List<FrontPayment>> GetAllFilteredAsync(Expression<Func<FrontPayment, bool>> expression)
		{
			return await _frontPaymentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<FrontPayment>> GetAllFrontPaymentsAsync()
		{
			return await _frontPaymentRepository.GetAllFrontPaymentsAsync();
		}

		public async Task<FrontPayment> GetByIdAsync(int id)
		{
			return await _frontPaymentRepository.GetByIdAsync(id);
		}

		public void Update(FrontPayment entity)
		{
			_frontPaymentRepository.Update(entity);
		}
	}
}
