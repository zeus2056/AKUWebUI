using AKUWebUI.Models.Exam;
using AKUWebUI.Models.ExamResult;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using BusinessLayer.Concrete.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	public class ExamResultsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreExamResultService _examResultService;
		private readonly IEFCoreRateService _rateService;
		private readonly IEFCoreExamService _examService;
		private readonly IEFCoreRateStudentService _rateStudentService;
		private readonly WebContext _context;
		private List<Error> errors = new();

		public ExamResultsController(UserManager<AppUser> userManager, IEFCoreExamResultService examResultService
			, IEFCoreRateService rateService, IEFCoreExamService examService,IEFCoreRateStudentService rateStudentService,WebContext context)
		{
			_userManager = userManager;
			_examResultService = examResultService;
			_rateService = rateService;
			_examService = examService;
			_rateStudentService = rateStudentService;
			_context = context;
		}
		private IActionResult AddError(Error error, string redirect)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect(redirect);
		}
		public async Task<IActionResult> Index(string slug, int? id)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher;
			if (slug == null || id == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre hatası..." }, "/Exams");
			var exam = await _examResultService.FindByIdAndSlugAsync(slug, (int)id);
			if (exam == null)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			ViewBag.RateStudents = exam.Rate.RateStudents.Where(r => r.StartRateDate < exam.ExamDate).ToList();
			return View(new CreateExamResultModel() { ExamId = (int)id, Slug = slug });
		}
		[HttpPost]
		public async Task<IActionResult> Index(CreateExamResultModel model)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher;
			var exam = await _examResultService.FindByIdAndSlugAsync(model.Slug, (int)model.ExamId);
			if (exam == null)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			ViewBag.RateStudents = exam.Rate.RateStudents.Where(r => r.StartRateDate < exam.ExamDate).ToList();
			if (!ModelState.IsValid)
				return View(model);
			if (model.ExamId == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			var rate = await _rateService.GetByIdAsync(exam.RateId);
			List<ExamResult> examResults = new();
			int i = 0;
			foreach (var item in exam.Rate.RateStudents.Where(r => r.StartRateDate <exam.ExamDate))
			{
				examResults.Add(new ExamResult()
				{
					ExamId = model.ExamId,
					RateId = exam.RateId,
					StudentId = item.StudentId,
					Note = model.Note[i],
					isStatus = exam.SuccessNote < model.Note[i] ? true : false
				})
				;
			
					item.SuccessState = true;
					item.GraduationState = exam.SuccessNote < model.Note[i] ? true : false;
				i++;
			}
			await _examResultService.AddRange(examResults);
				await _rateStudentService.UpdateRange(exam.Rate.RateStudents);
			return AddError(new Error() { AlertType = "success", Description = "Sınav Sonuçları Girildi..."},"/Exams");
		}
		[Authorize(Roles = $"{nameof(Rol.SuperAdmin)},${nameof(Rol.Admin)}")]
		public async Task<IActionResult> UpdateNote(string slug, int? id)
		{
			if (slug == null || id == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre hatası..." }, "/Exams");
			var exam = await _examResultService.FindByIdAndSlugAsync(slug, (int)id);
			if (exam == null)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			ViewBag.RateStudents = exam.Rate.RateStudents.Where(r => r.StartRateDate < exam.ExamDate).ToList();
			return View(new UpdateExamResultModel() { ExamId = (int)id, Slug = slug});
		}
		[Authorize(Roles = $"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
		[HttpPost]
		public async Task<IActionResult> UpdateNote(UpdateExamResultModel model)
		{
			var exam = await _examResultService.FindByIdAndSlugAsync(model.Slug, (int)model.ExamId);
			if (exam == null)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			ViewBag.RateStudents = exam.Rate.RateStudents.Where(r => r.StartRateDate < exam.ExamDate).ToList();
			if (!ModelState.IsValid)
				return View(model);
			if (model.ExamId == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, "/Exams");
			var examResults = await _examResultService.FindsByExamIdAsync(model.ExamId);
			int i = 0;
			foreach (var item in examResults)
			{
				item.Note = model.Note[i];
				item.isStatus = model.Note[i] > exam.SuccessNote ? true : false;
				exam.Rate.RateStudents[i].GraduationState = model.Note[i] > exam.SuccessNote ? true : false;
				i++;
			}
			_context.ExamResults.UpdateRange(examResults);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "success",Description= "Sınav Sonuçları Güncellendi..."},"/Exams");
		}
	}
}
