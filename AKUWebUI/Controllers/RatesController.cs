using AKUWebUI.Models.Rate;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles = $"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
	public class RatesController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreRateService _rateService;
		private readonly IEFCoreAgeGroupService _ageGroupService;
		private readonly IEFCoreStudentService _studentService;
		private readonly IEFCorePaymentTypeService _paymentTypeService;
		private readonly IEFCoreRateStudentService _rateStudentService;
		private readonly IEFCoreDiscountRateService _discountRateService;
		private readonly IEFCorePaymentService _paymentService;
		private readonly IEFCoreRatePaymentInfoService _ratePaymentInfoService;
		private readonly IEFCoreBranchService _branchService;
		private readonly IEFCoreFrontPaymentService _frontPaymentService;
		private List<Error> errors = new();
		public RatesController(UserManager<AppUser> userManager, IEFCoreRateService rateService, IEFCoreAgeGroupService ageGroupService, IEFCoreStudentService studentService, IEFCoreRateStudentService rateStudentService, IEFCoreBranchService branchService, IEFCoreDiscountRateService discountRateService, IEFCorePaymentTypeService paymentTypeService, IEFCorePaymentService paymentService, IEFCoreRatePaymentInfoService ratePaymentInfoService, IEFCoreFrontPaymentService frontPaymentService)
		{
			_userManager = userManager;
			_rateService = rateService;
			_ageGroupService = ageGroupService;
			_studentService = studentService;
			_rateStudentService = rateStudentService;
			_branchService = branchService;
			_discountRateService = discountRateService;
			_paymentTypeService = paymentTypeService;
			_paymentService = paymentService;
			_ratePaymentInfoService = ratePaymentInfoService;
			_frontPaymentService = frontPaymentService;
		}
		[NonAction]
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Index", "Rates");
		}
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync((User.Identity.Name));
			var rates = user.Role == Rol.SuperAdmin ? await _rateService.GetAllWithAgeGroup(0) : await _rateService.GetAllWithAgeGroup(user.BranchId);
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher ? true : false;
			return View(rates);
		}
		public async Task<IActionResult> CreateRate()
		{
			var ageGroups = (await _ageGroupService.GetAllAsync());
			if (ageGroups.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "There are not any AgeGroups..." });
			if ((await _userManager.FindByNameAsync(User.Identity.Name)).Role == Rol.SuperAdmin)
				ViewBag.Branchs = await _branchService.GetAllAsync();
			ViewBag.AgeGroups = ageGroups;
			return View();
		}
		public async Task<IActionResult> ChangeState(int? id)
		{
			if (id == null)
				return AddError(new Error() {AlertType ="danger", Description = "Parametre Hatası..." });
			var rate = await _rateService.GetByIdAsync((int)id);
			if (rate == null)
				return AddError(new Error() { AlertType="danger", Description = "Kur Bulunamadı..."});
			rate.RateState = !rate.RateState;
			_rateService.Update(rate);
			return AddError(new Error() { Description = "Kur Durumu Değiştirildi...", AlertType = "success"});
		}
		[HttpPost]
		public async Task<IActionResult> CreateRate(CreateRateModel model)
		
		{
			ViewBag.AgeGroups = await _ageGroupService.GetAllAsync();
			if ((await _userManager.FindByNameAsync(User.Identity.Name)).Role == Rol.SuperAdmin)
				ViewBag.Branchs = await _branchService.GetAllAsync();
			if (!ModelState.IsValid)
				return View(model);
			if (model.BranchId == 0)
			{
				ModelState.AddModelError("", "Branch is required...");
				return View(model);
			}
			var rates = await _rateService.GetAllAsync();
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var rate = await _rateService.GetAllFilteredAsync(r => r.RateName.ToLower() == model.RateName.ToLower() && r.BranchId == model.BranchId);
			if (rate.Count > 0)
			{
				ModelState.AddModelError("", "Kur ismi alınmış...");
				return View(model);
			}
			await _rateService.AddAsync(new Rate()
			{
				BranchId = (int)(user.Role == Rol.SuperAdmin ? model.BranchId : user.BranchId),
				AgeGroupId = model.AgeGroupId,
				RateDate = model.RateDate,
				RateName = model.RateName,
				RatePrice = model.RatePrice,
				RateStartDate = model.RateStartDate,
				RateState = true,
				Slug = model.RateName.Replace(" ", "-") + Guid.NewGuid(),
				Description = model.Description
			});
			return AddError(new Error() { AlertType = "success", Description = "Kur Eklendi..." });
		}
		public async Task<IActionResult> DeleteRate(string? slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug is required..." });
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			_rateService.Delete(rate);
			return AddError(new Error() { Description = "Rate has been deleted...", AlertType = "success" });
		}
		public async Task<IActionResult> EditRate(string slug)
		{
			ViewBag.AgeGroups = await _ageGroupService.GetAllAsync();
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug is required..." });
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			return View(new EditRateModel() { AgeGroupId = rate.AgeGroupId, RateDate = rate.RateDate, RateId = rate.RateId, RateName = rate.RateName, RatePrice = rate.RatePrice, RateStartDate = rate.RateStartDate, Description = rate.Description });
		}
		[HttpPost]
		public async Task<IActionResult> EditRate(EditRateModel model)
		{
			ViewBag.AgeGroups = await _ageGroupService.GetAllAsync();
			if (!ModelState.IsValid)
				return View(model);
			var rate = await _rateService.GetByIdAsync(model.RateId);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			var rates = await _rateService.GetAllFilteredAsync(r => r.RateId != model.RateId);
			if (rates.Count > 0)
				if (!rates.Any(r => r.RateName.Replace(" ", "-").ToLower() != model.RateName.Replace(" ", "-").ToLower()))
				{
					ModelState.AddModelError("", "RateName has been already used...");
					return View(model);
				}
			rate.RateStartDate = model.RateStartDate;
			rate.RateDate = model.RateDate;
			rate.AgeGroupId = model.AgeGroupId;
			rate.Slug = model.RateName.Replace(" ", "-");
			rate.RateName = model.RateName;
			rate.RatePrice = model.RatePrice;
			rate.Description = model.Description;
			_rateService.Update(rate);
			return AddError(new Error() { AlertType = "success", Description = "Rate has been edited..." });
		}
		public async Task<IActionResult> AddRateStudent(string? slug, int? branchId)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug is required..." });
			var rate = await _rateService.FindBySlugAsync(slug);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			if (rate.RateStartDate < DateTime.Now)
				return AddError(new Error() { AlertType = "danger", Description = "Kura kayıt tarihi geçmiş..."});
			var rateStudents = new List<Student>();
			var includeRate = await _rateService.GetRateWithRateStudentsAsync(rate.RateId);
			var students = await _studentService.GetAllStudentsWithIncludesAsync((int)branchId);
			foreach (var _student in students)
			{
				if (rate.RateStartDate > DateTime.Now)
				{
					if (_student.RateStudents.Count == 0 && (_student.Age >= rate.AgeGroup.StartAge && rate.AgeGroup.EndAge > _student.Age))
					{
						rateStudents.Add(_student);
					}
					else if (_student.RateStudents.Any(r => r.RateId != rate.RateId) && (_student.Age >= rate.AgeGroup.StartAge && rate.AgeGroup.EndAge > _student.Age))
					{
						rateStudents.Add(_student);
					}
				}
			}

			if (rateStudents.Count == 0)
				return AddError(new Error() { AlertType= "warning",Description = "Kursa Kaydetmek İçin Öğrenci Yok..."});
			var discounts = await _discountRateService.GetAllAsync();
			ViewBag.Discounts = discounts;
			ViewBag.RateStudents = rateStudents;
			return View(new CreateRateStudentModel() { RateId = rate.RateId });
		}
		[HttpPost]
		public async Task<IActionResult> AddRateStudent(CreateRateStudentModel model)
		{
			var rate = await _rateService.GetByIdAsync(model.RateId);
			var discounts = await _discountRateService.GetAllAsync();
			var paymentTypes = await _paymentTypeService.GetAllAsync();
			var rateStudents = new List<Student>();
			var includeRate = await _rateService.GetRateWithRateStudentsAsync(rate.RateId);
			var students = await _studentService.GetAllStudentsWithIncludesAsync(rate.BranchId);
			foreach (var _student in students)
			{
				if (_student.RateStudents.Count == 0)
				{
					rateStudents.Add(_student);
				}
				else if (_student.RateStudents.Any(r => r.RateId != rate.RateId))
				{
					rateStudents.Add(_student);
				}
			}
			ViewBag.Students = await _studentService.GetAllFilteredAsync(s => s.BranchId == rate.BranchId);
			ViewBag.Discounts = discounts;
			if (!ModelState.IsValid)
				return View(model);
			if (rate == null)
				return AddError(new Error() { AlertType = "danger", Description = "Rate has been not founded..." });
			var student = await _studentService.GetByIdAsync(model.StudentId);
			if (student == null)
				return AddError(new Error() { AlertType = "danger", Description = "Student has been not founded..." });
			var discount = await _discountRateService.GetByIdAsync((int)model.DiscountId);
			await _rateStudentService.AddAsync(new RateStudent()
			{
				RateId = model.RateId,
				StudentId = model.StudentId,
				RatePrice = model.DiscountId == 0 ? rate.RatePrice:  rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100)
			});
			var rateStudent = await _rateStudentService.LastRateStudentByAsync();
			await _ratePaymentInfoService.AddAsync(new RatePaymentInfo() { RateId = model.RateId, StudentId = model.StudentId, RateStudentId = rateStudent.RateStudentId});
			await _frontPaymentService.AddAsync(new FrontPayment() { Bakiye = 0, RateId = rate.RateId, RateStudentId = rateStudent.RateStudentId, RatePrice = rateStudent.RatePrice, RemaningPrice = rateStudent.RatePrice });
			return AddError(new Error() { AlertType = "danger", Description = "RateStudent has been added..." });
		}
		
	}
}
