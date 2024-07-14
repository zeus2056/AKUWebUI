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
	public class EFCorePaymentManager : IEFCorePaymentService
	{
		private readonly IEFCorePaymentRepository _paymentRepository;

		public EFCorePaymentManager(IEFCorePaymentRepository paymentRepository)
		{
			_paymentRepository = paymentRepository;
		}

		public async Task AddAsync(Payment entity)
		{
			await _paymentRepository.AddAsync(entity);
		}

		public void Delete(Payment entity)
		{
			_paymentRepository.Delete(entity);
		}

		public async Task<Payment> FindByIdsAsync(int rateId, int studentId, int rateStudenId)
		{
			return await _paymentRepository.FindByIdsAsync(rateId, studentId, rateStudenId);
		}

		public async Task<List<Payment>> FindPaymentsByIdAsync(int studentId)
		{
			return await _paymentRepository.FindPaymentsByIdAsync(studentId);
		}

		public async Task<List<Payment>> GetAllAsync()
		{
			return await _paymentRepository.GetAllAsync ();
		}

		public async Task<List<Payment>> GetAllFilteredAsync(Expression<Func<Payment, bool>> expression)
		{
			return await _paymentRepository.GetAllFilteredAsync(expression);
		}

		public async Task<List<Payment>> GetAllPaymentAsync(int branchId)
		{
			return await _paymentRepository.GetAllPaymentsAsync(branchId);
		}

		public async Task<List<Payment>> GetAllPaymentByRateStudentAsync(int studentId)
		{
			return await _paymentRepository.GetAllPaymentByRateStudentAsync(studentId);
		}

		public async Task<Payment> GetByIdAsync(int id)
		{
			return await _paymentRepository.GetByIdAsync(id);
		}

		public void Update(Payment entity)
		{
			_paymentRepository.Update(entity);
		}
	}
}
