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
	public class EFCoreRatePaymentInfoManager : IEFCoreRatePaymentInfoService
	{
		private readonly IEFCoreRatePaymentInfoRepository _ratePaymentRepository;

		public EFCoreRatePaymentInfoManager(IEFCoreRatePaymentInfoRepository ratePaymentRepository)
		{
			_ratePaymentRepository = ratePaymentRepository;
		}

		public async Task AddAsync(RatePaymentInfo entity)
		{
			await _ratePaymentRepository.AddAsync(entity);
		}

		public void Delete(RatePaymentInfo entity)
		{
			_ratePaymentRepository.Delete(entity);
		}

		public async Task<List<RatePaymentInfo>> FindByIdsAsync(int studentId)
		{
			return await _ratePaymentRepository.FindByIdsAsync(studentId);
		}

		public async Task<List<RatePaymentInfo>> GetAllAsync()
		{
			return await _ratePaymentRepository.GetAllAsync();
		}

		public async Task<List<RatePaymentInfo>> GetAllFilteredAsync(Expression<Func<RatePaymentInfo, bool>> expression)
		{
			return await _ratePaymentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<RatePaymentInfo> GetByIdAsync(int id)
		{
			return await _ratePaymentRepository.GetByIdAsync(id);
		}

		public void Update(RatePaymentInfo entity)
		{
			_ratePaymentRepository.Update(entity);
		}
	}
}
