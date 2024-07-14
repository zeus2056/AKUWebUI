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
	public class EFCoreHistoryPaymentManager : IEFCoreHistoryPaymentService
	{
		private readonly IEFCoreHistoryPaymentRepository _historyPaymentRepository;

		public EFCoreHistoryPaymentManager(IEFCoreHistoryPaymentRepository historyPaymentRepository)
		{
			_historyPaymentRepository = historyPaymentRepository;
		}

		public async Task AddAsync(HistoryPayment entity)
		{
			 await _historyPaymentRepository.AddAsync(entity);
		}

		public void Delete(HistoryPayment entity)
		{
			_historyPaymentRepository.Delete(entity);
		}

		public async Task<List<HistoryPayment>> FindByIdAsync(int studentId)
		{
			return await _historyPaymentRepository.FindByIdAsync(studentId);
		}

		public async Task<List<HistoryPayment>> GetAllAsync()
		{
			return await _historyPaymentRepository.GetAllAsync();
		}

		public async Task<List<HistoryPayment>> GetAllFilteredAsync(Expression<Func<HistoryPayment, bool>> expression)
		{
			return await _historyPaymentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<HistoryPayment> GetByIdAsync(int id)
		{
			return await _historyPaymentRepository.GetByIdAsync(id);
		}

		public void Update(HistoryPayment entity)
		{
			_historyPaymentRepository.Update(entity);
		}
	}
}
