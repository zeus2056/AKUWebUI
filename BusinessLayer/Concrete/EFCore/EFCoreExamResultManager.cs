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
	public class EFCoreExamResultManager : IEFCoreExamResultService
	{
		private readonly IEFCoreExamResultRepository _examResultRepository;

		public EFCoreExamResultManager(IEFCoreExamResultRepository examResultRepository)
		{
			_examResultRepository = examResultRepository;
		}

		public async Task AddAsync(ExamResult entity)
		{
			await _examResultRepository.AddAsync(entity);
		}

		public async Task AddRange(List<ExamResult> examResults)
		{
			await _examResultRepository.AddRange(examResults);
		}

		public void Delete(ExamResult entity)
		{
			_examResultRepository.Delete(entity);
		}

		public async Task<ExamResult> FindByExamIdAsync(int examId)
		{
			return await _examResultRepository.FindByExamIdAsync(examId);
		}

		public async Task<Exam> FindByIdAndSlugAsync(string slug, int id)
		{
			return await _examResultRepository.FindByIdAndSlugAsync(slug, id);
		}

		public async Task<List<ExamResult>> FindsByExamIdAsync(int examId)
		{
			return await _examResultRepository.FindsByExamIdAsync(examId);
		}

		public async Task<List<ExamResult>> GetAllAsync()
		{
			return await _examResultRepository.GetAllAsync();
		}

		public async Task<List<ExamResult>> GetAllFilteredAsync(Expression<Func<ExamResult, bool>> expression)
		{
			return await _examResultRepository.GetAllFilteredAsync(expression);
		}

		public async Task<ExamResult> GetByIdAsync(int id)
		{
			return await _examResultRepository.GetByIdAsync(id);
		}

		public async Task<List<ExamResult>> GetExamResultsByStudentIdAsync(int studentId)
		{
			return await _examResultRepository.GetExamResultsByStudentIdAsync(studentId);
		}

		public void Update(ExamResult entity)
		{
			_examResultRepository.Update(entity);
		}

		public  void UpdateRange(List<ExamResult> examResults)
		{
			 _examResultRepository.UpdateRange(examResults);
		}
	}
}
