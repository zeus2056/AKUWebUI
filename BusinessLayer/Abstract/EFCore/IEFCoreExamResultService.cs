using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreExamResultService : IEFCoreGenericService<ExamResult>
	{
		Task<Exam> FindByIdAndSlugAsync(string slug, int id);
		Task<ExamResult> FindByExamIdAsync(int examId);
		Task<List<ExamResult>> FindsByExamIdAsync(int examId);
		void UpdateRange(List<ExamResult> examResults);
		Task AddRange(List<ExamResult> examResults);
		Task<List<ExamResult>> GetExamResultsByStudentIdAsync(int studentId);
	}
}
