using BusinessLayer.Abstract.EFCore;
using Microsoft.AspNetCore.Mvc;

namespace AKUWebUI.ViewComponents
{
	public class GetRateStudentsComponent : ViewComponent
	{
		private readonly IEFCoreRateService _rateService;
		private readonly IEFCoreStudentService _studentService;
		private readonly IEFCoreRateStudentService _rateStudentService;

		public GetRateStudentsComponent(IEFCoreRateService rateService, IEFCoreStudentService studentService,
			IEFCoreRateStudentService rateStudentService)
		{
			_rateService = rateService;
			_studentService = studentService;
			_rateStudentService = rateStudentService;
		}

		public async Task<IViewComponentResult> InvokeAsync(int rateId,int branchId)
		{
			var rate = await _rateService.GetByIdAsync(rateId);
			var students = await _studentService.GetAllByBranchAndRateId(rate.RateId, rate.BranchId);
			foreach (var student in students)
			{
				foreach (var item in student.Rates)
				{
					if (item.RateId == rate.RateId)
					{
						students.Remove(student);
					}
				}
			}	
			return View(students);
		}
	}
}
