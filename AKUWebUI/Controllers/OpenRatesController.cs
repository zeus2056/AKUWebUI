using AKUWebUI.Models.ChangeRate;
using AKUWebUI.Models.ChangeRate;
using AKUWebUI.Models.OpenRate;
using AKUWebUI.Models.Rate;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles = $"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
	public class OpenRatesController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly WebContext _context;
		private readonly IEFCoreRateService _rateService;
		private readonly IEFCoreRateStudentService _rateStudentService;
		private readonly IEFCoreAgeGroupService _ageGroupService;
		private readonly IEFCoreExamResultService _examResultService;
		private readonly IEFCoreRatePaymentInfoService _ratePaymentInfoService;
		private readonly IEFCoreFrontPaymentService _frontPaymentService;
		private readonly IEFCoreStudentService _studentService;
		private List<Error> errors = new();

		public OpenRatesController(UserManager<AppUser> userManager, IEFCoreRateService rateService, IEFCoreRateStudentService rateStudentService, IEFCoreAgeGroupService ageGroupService, IEFCoreExamResultService examResultService, IEFCoreRatePaymentInfoService ratePaymentInfoService, IEFCoreFrontPaymentService frontPaymentService,IEFCoreStudentService studentService,
			WebContext context)
		{
			_userManager = userManager;
			_rateService = rateService;
			_rateStudentService = rateStudentService;
			_ageGroupService = ageGroupService;
			_examResultService = examResultService;
			_ratePaymentInfoService = ratePaymentInfoService;
			_frontPaymentService = frontPaymentService;
			_studentService = studentService;
			_context = context;
		}

		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("/OpenRates");
		}
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = user.Role < Rol.Teacher ? true : false;
			var rates = await _rateService.GetAllOpenRatesWithAgeGroupAsync(user.Role == Rol.SuperAdmin ? 0 : user.BranchId);
			return View(rates);
		}
		public async Task<IActionResult> EditState(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug boş geçilemez..." });
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur bulunamadı..." });
			rate.RateState = !rate.RateState;
			_rateService.Update(rate);
			return AddError(new Error() { AlertType = "success", Description = "Kur Durumu Değişti..." });
		}
		public async Task<IActionResult> RemoveStudent(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug boş geçilemez..." });
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur bulunamadı..." });
			ViewBag.RateStudents = await _rateStudentService.FindByRateIdAsync(rate.RateId);
			if (ViewBag.RateStudents.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Kurda kayıtlı öğrenci yok..." });
			return View(new RemoveStudentModel() { RateId = rate.RateId });
		}
		public async Task<IActionResult> EditRate(string slug)
		{
			if (slug == null)
				return AddError(new Error()
				{
					 AlertType = "danger",
					 Description = "Parametre hatası var..."
				});
			var rate = await _context.Rates.AsNoTracking().FirstOrDefaultAsync(r => r.Slug ==slug);
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
			ViewBag.Role = user.Role;
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..."});
			if(user.Role != Rol.SuperAdmin)
				return View(new UpdateOpenRateModel() { AgeGroupId = rate.AgeGroupId, RateId = rate.RateId, RateName = rate.RateName, RatePrice = rate.RatePrice, RateDate = rate.RateDate, Description = rate.Description });
			return View(new UpdateOpenRateModel() { AgeGroupId = rate.AgeGroupId, RateId = rate.RateId, RateName = rate.RateName, RatePrice = rate.RatePrice, RateDate = rate.RateDate, Description = rate.Description, BranchId = rate.BranchId });
		}
		[HttpPost]
		public async Task<IActionResult> EditRate(UpdateOpenRateModel model)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = user.Role;
			ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
			if (!ModelState.IsValid)
				return View(model);
			var rate = await _context.Rates.FirstOrDefaultAsync(r => r.RateId == model.RateId);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..."});
			if (rate.AgeGroupId == 0)
			{
				ModelState.AddModelError("","Yaş Grubu Boş Geçilemez...");
				return View(model);
			}
			rate.RateName = model.RateName;
			rate.Slug = model.RateName.Replace(" ", "-");
			rate.RateDate = model.RateDate;
			rate.RatePrice = model.RatePrice;
			rate.AgeGroupId = model.AgeGroupId;
			rate.Description = model.Description;
			if (model.BranchId != null)
				rate.BranchId = (int)model.BranchId;
			_context.Rates.Update(rate);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType ="success", Description = "Kur Güncellendi..."});
			
		}
		[HttpPost]
		public async Task<IActionResult> RemoveStudent(RemoveStudentModel model)
		{
			var rate = await _rateService.GetByIdAsync(model.RateId);
			ViewBag.RateStudents = await _rateStudentService.FindByRateIdAsync(rate.RateId);
			if (!ModelState.IsValid)
				return View(model);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..." });
			var rateStudent = await _rateStudentService.GetByIdAsync(model.RateStudentId);
			if (rateStudent == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
			_rateStudentService.Delete(rateStudent);
			return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Silindi..." });
		}
		public async Task<IActionResult> AddRateStudent(string slug, int branchId)
		{
			ViewBag.Rate = await _rateService.FindBySlugAsync(slug);
			var students = await _studentService.GetAllByBranchAndRateId((ViewBag.Rate as Rate).AgeGroupId, (ViewBag.Rate as Rate).BranchId);
			var rateStudents = new List<Student>();
			if (students.Count == 0)
				return AddError(new Error() { AlertType = "danger",Description = "Eklenecek Ogrenci Bulunamadı..."});
		
			foreach (var student in students)
			{
				rateStudents.Add(student);
				foreach (var item in student.RateStudents)
				{
					if (!student.RateStudents.All(r => r.State))
					{
						rateStudents.Remove(student);
						break;
					}
					//if (item.RateId == (ViewBag.Rate as Rate).RateId)
					//{
					//	rateStudents.Remove(student);
					//	break;
					//}
				}
			}
			if (rateStudents.Count == 0)
				return AddError(new Error() { AlertType ="danger",Description = "Eklenecek Ogrenci Bulunamadı..."});
			ViewBag.Students = rateStudents;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> AddRateStudent(CreateRateStudentModel model)
		{
			ViewBag.Rate = await _rateService.GetByIdAsync(model.RateId);
			var students = await _studentService.GetAllByBranchAndRateId((ViewBag.Rate as Rate).AgeGroupId, (ViewBag.Rate as Rate).BranchId);
			var rateStudents = new List<Student>();
			if (students.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Eklenecek Ogrenci Bulunamadı..." });
			if (model.StartRateDate.Date < DateTime.Now.Date)
			{
				ModelState.AddModelError("","Tarih Bilgisi Hatalıdır...");
				return View(model);
			}
			foreach (var student in students)
			{
				rateStudents.Add(student);
				foreach (var item in student.RateStudents)
				{
                    if (!student.RateStudents.All(r => r.State))
                    {
                        rateStudents.Remove(student);
                        break;
                    }
                    //if (item.RateId == (ViewBag.Rate as Rate).RateId)
                    //{
                    //	rateStudents.Remove(student);
                    //	break;
                    //}
                }
			}
			if (rateStudents.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Eklenecek Ogrenci Bulunamadı..." });
			ViewBag.Students = rateStudents;
			if (!ModelState.IsValid)
				return View(model);
			var rate = await _rateService.GetByIdAsync(model.RateId);
			var _student = await _studentService.GetByIdAsync(model.StudentId);
			var discount = await _context.DiscountRates.AsNoTracking().FirstOrDefaultAsync(d=> d.DiscountRateId == model.DiscountId);
			await _rateStudentService.AddAsync(new RateStudent()
			{
				RateId = model.RateId,
				RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100),
				RegisterDate = DateTime.Now,
				StudentId = model.StudentId,
				StartRateDate = model.StartRateDate,
				EndRateDate = model.StartRateDate.AddDays(TimeSpan.FromDays((rate.RateDate / 3 + 1) * 7).TotalDays)
			});
			var rateStudent = await _context.RateStudents.OrderBy(r => r.RateStudentId).LastOrDefaultAsync();
			await _frontPaymentService.AddAsync(new FrontPayment() { RateId = rate.RateId, RemaningPrice = 0, Bakiye = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RateStudentId = rateStudent.RateStudentId});
			await _ratePaymentInfoService.AddAsync(new RatePaymentInfo() { RateId = rate.RateId, RateStudentId = rateStudent.RateStudentId, StudentId = model.StudentId});
			return AddError(new Error() { AlertType = "success", Description = "Öğrenci Eklendi..." });
		}
		public async Task<IActionResult> RepeatRate(int id, string slug)
		{
			var rate = await _context.Rates.AsNoTracking().FirstOrDefaultAsync(r => r.RateId == id);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description ="Kur Bulunamadı..."});
			var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Slug == slug);
			if (student == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..."});
			return View(new RepeatRateModel() {  RateId = rate.RateId, StudentId = student.StudentId});
		}
		[HttpPost]
		public async Task<IActionResult> RepeatRate(RepeatRateModel model)
		{
			if (!ModelState.IsValid)
				return View();
			var rate = await _rateService.GetByIdAsync(model.RateId);
			if (model.DiscountId == 0)
			{
				ModelState.AddModelError("","Lütfen İndirim Seçiniz...");
				return View(model);
			}
			var discount = await _context.DiscountRates.FindAsync(model.DiscountId);
			var rateStudent = await _context.RateStudents.FirstOrDefaultAsync(r => r.StudentId ==model.StudentId && r.RateId == model.RateId && !r.State);
			if (rateStudent == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..."});
			rateStudent.State = true;
			_context.RateStudents.Update(rateStudent);
			await _context.SaveChangesAsync();
			await _rateStudentService.AddAsync(new RateStudent() { RateId =model.RateId, StudentId = model.StudentId, RegisterDate = DateTime.Now, StartRateDate = model.StartRateDate, EndRateDate = model.StartRateDate.AddDays(TimeSpan.FromDays((rate.RateDate / 3 + 1) * 7).TotalDays) , RatePrice  = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100)});
			var _rateStudent = await _context.RateStudents.OrderBy(r => r.RateStudentId).AsNoTracking().LastOrDefaultAsync();
			await _context.RatePaymentInfos.AddAsync(new RatePaymentInfo() { RateStudentId = _rateStudent.RateStudentId, RateId = _rateStudent.RateId, StudentId = _rateStudent.StudentId});
			await _context.FrontPayments.AddAsync(new FrontPayment() { RateId = rate.RateId, RemaningPrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), Bakiye = 0, RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), 
				RateStudentId = _rateStudent.RateStudentId } );
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "success", Description ="Kur Tekrarlama Eklendi..."});
		}
		public async Task<IActionResult> ShowStudents(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug is required..." });
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher;
			return View(rate.RateStudents.Where(r => !r.State).ToList());
		}
		public async Task<IActionResult> DeleteStudent(int id, string slug)
		{
			var rate = await _rateService.GetByIdAsync(id);
			var student = await _studentService.FindBySlugAsync(slug);
			var rateStudent = await _context.RateStudents.Include(r => r.Payments).FirstOrDefaultAsync(r => r.RateId == rate.RateId && r.StudentId == student.StudentId && !r.State);
			var frontPayment = await _context.FrontPayments.FirstOrDefaultAsync(r => r.RateId == id && r.RateStudentId == rateStudent.RateStudentId);
			var ratePaymentInfo = await _context.RatePaymentInfos.FirstOrDefaultAsync(r => r.RateId == id && r.StudentId == student.StudentId);
			rateStudent.State = true;
			rateStudent.SuccessState = true;
			rateStudent.GraduationState = false;
			frontPayment.State = true;
			var payment = await _context.Payments.FirstOrDefaultAsync(p => !p.State && p.RateStudentId == rateStudent.RateStudentId && p.StudentId == student.StudentId && p.RateId == rate.RateId);
			if (payment != null)
				payment.State = true;
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "success",Description = "Öğrenci Sİlindi..."});
		}
		public async Task<IActionResult> ChangeRate(int? id, string slug)
		{
			if (id == null || slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var rateStudent = await _rateStudentService.FindByIdandSlugAsync((int)id, slug);
			if (rateStudent == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
			var ageGroup = await _ageGroupService.GetByIdAsync(rateStudent.Rate.AgeGroupId);
			if (ageGroup == null)
				return AddError(new Error() { AlertType = "danger", Description = "Yaş Grubu Bulunamadı..." });
			var rates = (await _rateService.FindByAgeandBranchAsync(ageGroup.AgeGroupId, rateStudent.Student.BranchId)).Where(r => r.RateId != id).ToList();
			if (rates.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "Başka Kur Olamadığı için kur değiştirme yapamazsınız..." });
			ViewBag.Rates = rates.Where(r => r.RateState).ToList();
			return View(new UpdateChangeRateModel() { AgeGroupId = ageGroup.AgeGroupId, RateStudentId = rateStudent.RateStudentId, BranchId = rateStudent.Rate.BranchId, RateId = rateStudent.RateId });
		}
		[HttpPost]
		public async Task<IActionResult> ChangeRate(UpdateChangeRateModel model)
		{
			var rates = (await _rateService.FindByAgeandBranchAsync(model.AgeGroupId, model.BranchId)).Where(r => r.RateId != model.RateId).ToList();
			ViewBag.Rates = rates.Where(r => r.RateState).ToList();
			if (!ModelState.IsValid)
				return View(model);
			var rate = await _rateService.GetByIdAsync(model.RateId);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..." });
			var rateStudent = await _rateService.FindIncludeByIdAsync(model.RateStudentId);
			if (rateStudent == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
			if (!(rateStudent.SuccessState && rateStudent.GraduationState))
				return AddError(new Error() { AlertType = "danger", Description = "İlk önce kuru tamamlamalısınız..." });
			if (rateStudent.RateId == model.RateId)
			{
				ModelState.AddModelError("", "Öğrenci zaten bu kurda...");
				return View(model);
			}
			rateStudent.State = true;
			_context.RateStudents.Update(rateStudent);
			await _context.SaveChangesAsync();
			var discount = await _context.DiscountRates.FirstOrDefaultAsync(d => d.DiscountRateId == model.DiscountId);
			await _rateStudentService.AddAsync(new RateStudent() { RateId = rate.RateId, RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RegisterDate = DateTime.Now, StudentId = rateStudent.StudentId, StartRateDate = model.StartRateDate, EndRateDate = model.StartRateDate.AddDays(TimeSpan.FromDays((rate.RateDate / 3 + 1) * 7).TotalDays) });
			
			var _rateStudent = await _context.RateStudents.Include(r => r.Student).OrderBy(r => r.RateStudentId).LastOrDefaultAsync();
			await _ratePaymentInfoService.AddAsync(new RatePaymentInfo() { RateId = rate.RateId, RateStudentId = _rateStudent.RateStudentId, StudentId = _rateStudent.Student.StudentId });
			await _frontPaymentService.AddAsync(new FrontPayment() { Bakiye = 0, RateId = rate.RateId, RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RateStudentId = _rateStudent.RateStudentId, RemaningPrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100) });
			return AddError(new Error() { AlertType = "success", Description = "Kur Başarılı bir şekilde değiştirildi..." });
		}
	}
}
