


using AKUWebUI.Models;
using AKUWebUI.Models.ChangeRate;
using AKUWebUI.Models.OpenRate;
using AKUWebUI.Models.Student;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rotativa;
using AKUWebUI.Models.Filtered;

namespace AKUWebUI.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IEFCoreStudentService _studentService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEFCoreBranchService _branchService;
        private readonly IEFCoreRateStudentService _rateStudentService;
        private readonly IEFCoreExamResultService _eFCoreExamResultService;
        private readonly IEFCoreRatePaymentInfoService _paymentInfoService;
        private readonly IEFCoreParentService _parentService;
        private readonly IEFCorePaymentTypeService _paymentTypeService;
        private readonly IEFCoreBankService _bankService;
        private readonly IEFCoreRateService _rateService;
        private readonly IEFCorePaymentService _paymentService;
        private readonly IEFCoreHistoryPaymentService _historyPaymentService;
        private readonly IEFCoreFrontPaymentService _frontPaymentService;
        private readonly WebContext _context;
        private List<Error> errors = new();

        public StudentsController(IEFCoreStudentService studentService, UserManager<AppUser> userManager, IEFCoreBranchService branchService, IEFCoreRateStudentService rateStudentService, IEFCoreExamResultService eFCoreExamResultService, IEFCorePaymentService paymentService, IEFCoreRatePaymentInfoService paymentInfoService, IEFCoreBankService bankService, IEFCoreRateService rateService, IEFCorePaymentTypeService paymentTypeService, IEFCoreParentService parentService
            , IEFCoreHistoryPaymentService historyPaymentService, IEFCoreFrontPaymentService frontPaymentService, WebContext context)
        {
            _studentService = studentService;
            _userManager = userManager;
            _branchService = branchService;
            _rateStudentService = rateStudentService;
            _eFCoreExamResultService = eFCoreExamResultService;
            _paymentService = paymentService;
            _paymentInfoService = paymentInfoService;
            _bankService = bankService;
            _rateService = rateService;
            _paymentTypeService = paymentTypeService;
            _parentService = parentService;
            _historyPaymentService = historyPaymentService;
            _frontPaymentService = frontPaymentService;
            _context = context;

        }

        private IActionResult AddError(Error error, string? slug)
        {
            errors.Add(error);
            TempData["Errors"] = JsonConvert.SerializeObject(errors);
            if (slug != null)
                return RedirectToAction("Details", "Students", new
                {
                    name = slug
                });
            else
                return Redirect("/Students");
        }

        public async Task<IActionResult> Index()
        {

            var user = await _userManager.FindByNameAsync(User?.Identity?.Name);
            var roles = await _userManager.GetRolesAsync(user);
            var isSuperAdmin = roles.Any(r => r == Rol.SuperAdmin.ToString());
            ViewBag.Role = roles.Any(r => r == Rol.SuperAdmin.ToString() || r == Rol.Admin.ToString());
            if (!isSuperAdmin)
            {
                var students = await _context.Students.Include(s => s.Branch).Where(r => r.BranchId == user.BranchId).ToListAsync();
                return View(students);
            }

            var _students = await _context.Students.Include(s => s.Branch).ToListAsync();
            return View(_students);
        }
        [Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
        public async Task<IActionResult> CreateStudent()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Role = await _userManager.IsInRoleAsync(user, Rol.SuperAdmin.ToString());
            if (ViewBag.Role)
                ViewBag.Branches = await _branchService.GetAllAsync();
            return View();
        }
        [Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Role = await _userManager.IsInRoleAsync(user, Rol.SuperAdmin.ToString());
            if (ViewBag.Role)
                ViewBag.Branches = await _branchService.GetAllAsync();
            if (!ModelState.IsValid)
                return View(model);
            //var validate = await _context.Students.FirstOrDefaultAsync(s => (s.Name + s.Surname).ToLower() == (model.Name.ToLower() + model.Surname.ToLower()));
            //if (validate != null)
            //{
            //	ModelState.AddModelError("","Böyle bir öğrenci zaten var...");
            //	return View(model);
            //}
            var tcValidate = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.TC == model.TC);
            if (tcValidate != null)
            {
                ModelState.AddModelError("", "TC Kullanılıyor....");
                return View(model);
            }
            //var phoneValidate = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Phone == model.Phone);
            //if (phoneValidate != null)
            //{

            //	ModelState.AddModelError("","Telefon Numarası Zaten Kullanılıuor...");
            //	return View(model);
            //}
            if (model.AgeGroupId == 0)
            {
                ModelState.AddModelError("", "Lütfen Yaş Grubu Seçiniz...");
                return View(model);
            }
            if (model.BranchId == 0)
            {
                ModelState.AddModelError("", "Lütfen Şube Seçiniz...");
                return View(model);
            }
            if (model.StartRateDate <= DateTime.Now)
            {
                ModelState.AddModelError("", "Lütfen Geçerli Bir Başlangıç Tarihi Seçiniz...");
                return View(model);
            }
            if (model.DiscountId == 0)
            {
                ModelState.AddModelError("", "İndirim Oranı Zorunludur....");
                return View(model);
            }
            var discount = await _context.DiscountRates.AsNoTracking().FirstOrDefaultAsync(d => d.DiscountRateId == model.DiscountId);

            TimeSpan ageTime = DateTime.Now - model.Date;
            var uploadsFolder = Path.Combine("wwwroot/", "Images");
            var student = new Student()
            {
                BranchId = model.BranchId,
                Age = (int)(ageTime.TotalDays / 365),
                Date = model.Date,
                Gender = model.Gender,
                Name = model.Name,
                Phone = model.Phone,
                Surname = model.Surname,
                TC = model.TC,
                Slug = model.Name.Replace(" ", "-") + "-" + model.Surname,
                Class = model.Class,
                SchoolName = model.SchoolName
            };
            if (model.ImagePath != null)
            {
                var imagePath = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var filePath = Path.Combine(uploadsFolder, imagePath);
                using (var stream = new System.IO.FileStream(filePath, FileMode.Create))
                {
                    await model.ImagePath.CopyToAsync(stream);
                }
                student.ImagePath = imagePath;
            }
            if (model.ExamPath != null)
            {
                var examPath = Guid.NewGuid().ToString() + "_" + model.ExamPath.FileName;
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var examFilePath = Path.Combine(uploadsFolder, examPath);
                using (var stream = new System.IO.FileStream(examFilePath, FileMode.Create))
                {
                    await model.ExamPath.CopyToAsync(stream);
                }
                student.ExamPath = examPath;
            }
            if (!ViewBag.Role)
                student.BranchId = user.BranchId;
            var errors = new List<Error>();
            var rate = await _context.Rates.Include(r => r.AgeGroup).AsNoTracking().FirstOrDefaultAsync(r => r.AgeGroupId == model.AgeGroupId && r.RateName == model.RateName);
            if (rate == null)
            {
                ModelState.AddModelError("", "Öğrenciye ait uygun kur bulunamadı...");
                return View(model);
            }
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            await _context.RateStudents.AddAsync(new RateStudent()
            {
                RateId = rate.RateId,
                StudentId = student.StudentId,
                RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100),
                RegisterDate = DateTime.Now,
                StartRateDate = model.StartRateDate,
                EndRateDate = model.StartRateDate.AddDays(TimeSpan.FromDays((rate.RateDate / 3 + 1) * 7).TotalDays)
            });
            ;
            await _context.SaveChangesAsync();
            var rateStudent = await _context.RateStudents.OrderBy(r => r.RateStudentId).LastOrDefaultAsync();
            await _context.FrontPayments.AddAsync(new FrontPayment() { Bakiye = 0, RateId = rate.RateId, RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RemaningPrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RateStudentId = rateStudent.RateStudentId });
            await _context.RatePaymentInfos.AddAsync(new RatePaymentInfo() { RateId = rate.RateId, RateStudentId = rateStudent.RateStudentId, StudentId = student.StudentId });
            await _context.SaveChangesAsync();

            return RedirectToAction("CreateParent","Parents",new
            {
                slug = student.Slug
            });
        }
        [Authorize(Roles = $"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
        public async Task<IActionResult> ChangeState(string name)
        {
            if (name == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var student = await _studentService.FindBySlugAsync(name);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı.." }, null);
            student.State = !student.State;
            _studentService.Update(student);
            return AddError(new Error() { Description = "Öğrenci durum değiştirildi..", AlertType = "success" }, null);
        }
        public async Task<IActionResult> Details(string? name)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Role = (await _userManager.GetRolesAsync(user)).Any(u => u != Rol.Teacher.ToString());
            var errors = new List<Error>();
            if (name == null)
                return Redirect("/Students");
            var student = await _studentService.GetUserBySlugAsync(name);
            if (student == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Students");
            }
            return View(student);
        }
        [Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
        public async Task<IActionResult> EditStudent(string? slug)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Role = await _userManager.IsInRoleAsync(user, Rol.SuperAdmin.ToString());
            if (ViewBag.Role)
                ViewBag.Branches = await _branchService.GetAllAsync();
            var errors = new List<Error>();
            if (slug == null)
                return Redirect("/Students");
            var student = await _studentService.GetUserBySlugAsync(slug);
            if (student == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Students");
            }
            return View(new UpdateStudentModel()
            {
                BranchId = student.BranchId,
                Date = student.Date,
                Gender = student.Gender,
                Name = student.Name,
                Phone = student.Phone,
                Surname = student.Surname,
                StudentId = student.StudentId,
                TC = student.TC,
                Class = student.Class,
                SchoolName = student.SchoolName
            });

        }
        [Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
        [HttpPost]
        public async Task<IActionResult> EditStudent(UpdateStudentModel model)
        {
            var errors = new List<Error>();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var student = await _studentService.GetByIdAsync(model.StudentId);
            ViewBag.Role = await _userManager.IsInRoleAsync(user, Rol.SuperAdmin.ToString());
            if (ViewBag.Role)
            {
                ViewBag.Branches = await _branchService.GetAllAsync();
                student.BranchId = model.BranchId;
            }

            if (!ModelState.IsValid)
                return View(model);
            if (model.BranchId == 0)
            {
                ModelState.AddModelError("", "Branch is required");
                return View(model);
            }

            if (student == null)
            {
                ModelState.AddModelError("", "Student has been not found..");
                return View(model);
            }
            var tcValidate = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId != model.StudentId && s.TC == model.TC);
            if (tcValidate != null)
            {
                ModelState.AddModelError("", "TC zaten kullanılıyor...");
                return View(model);
            }
            var phoneValidate = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId != student.StudentId && s.Phone == model.Phone);
            if (phoneValidate != null)
            {
                ModelState.AddModelError("", "Telefon Numarası Kullanılıyor....");
                return View(model);
            }
            TimeSpan ageTime = DateTime.Now - model.Date;
            student.Age = (int)(ageTime.TotalDays / 365);
            student.Name = model.Name;
            student.Surname = model.Surname;
            student.Date = model.Date;
            student.TC = model.TC;
            student.Phone = model.Phone;
            student.Gender = model.Gender;
            student.Slug = model.Name.Replace(" ", "-") + "-" + model.Surname;
            student.Class = model.Class;
            student.SchoolName = model.SchoolName;

            if (model.ImagePath != null)
            {
                var filePath = Path.Combine("wwwroot/Images/", student.ImagePath);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                var uploadsFolder = Path.Combine("wwwroot/", "Images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var imagePath = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;

                var _filePath = Path.Combine(uploadsFolder, imagePath);


                using (var stream = new System.IO.FileStream(_filePath, FileMode.Create))
                {
                    await model.ImagePath.CopyToAsync(stream);
                }
                student.ImagePath = imagePath;
            }
            if (model.ExamPath != null)
            {
                var filePath = Path.Combine("wwwroot/Images/", student.ExamPath);

                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                var uploadsFolder = Path.Combine("wwwroot/", "Images");

                var examPath = Guid.NewGuid().ToString() + "_" + model.ExamPath.FileName;
                var examFilePath = Path.Combine(uploadsFolder, examPath);
                using (var stream = new System.IO.FileStream(examFilePath, FileMode.Create))
                {
                    await model.ExamPath.CopyToAsync(stream);
                }
                student.ExamPath = examPath;
            }
            _studentService.Update(student);
            return AddError(new Error() { AlertType = "success", Description = "Öğrenci Güncellendi..." }, student.Slug);
        }
        public async Task<IActionResult> ShowRates(string slug)
        {
            if (slug == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var student = await _studentService.FindBySlugAsync(slug);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var rates = await _rateStudentService.GetStudentRatesAsync(student.StudentId);
            return View(rates);
        }
        public async Task<IActionResult> ShowRatesRapor(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType="danger", Description = "Parametre Hatası..."},null);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci bulunamadı..."},null);
            var rates = await _rateStudentService.GetStudentRatesAsync(student.StudentId);
            return new Rotativa.AspNetCore.ViewAsPdf(rates);
        }  
        public async Task<IActionResult> ShowExams(string slug)
        {
            if (slug == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var student = await _studentService.FindBySlugAsync(slug);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var examResults = await _context.Exams.Include(e => e.Student).Include(e => e.ExamResult).Include(r => r.RateStudent).Include(r => r.Rate).ThenInclude(r => r.AgeGroup).AsNoTracking().Where(e => e.StudentId == student.StudentId).ToListAsync();
            //ViewBag.LastExamId = examResults?.OrderBy(e => e.ExamId)?.LastOrDefault()?.ExamId;
            ViewBag.LastExamId = (await _context.RateStudents.Include(r => r.Exam).AsNoTracking().FirstOrDefaultAsync(r => r.StudentId == student.StudentId && !r.State))?.Exam?.ExamId;
            return View(examResults);
        }
        public async Task<IActionResult> ShowExamsRapor(string slug)
        {
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
			var student = await _studentService.FindBySlugAsync(slug);
			if (student == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
			var examResults = await _context.Exams.Include(e => e.Student).Include(e => e.ExamResult).Include(r => r.RateStudent).Include(r => r.Rate).ThenInclude(r => r.AgeGroup).AsNoTracking().Where(e => e.StudentId == student.StudentId).ToListAsync();
			return new Rotativa.AspNetCore.ViewAsPdf(examResults);
		}
        public async Task<IActionResult> PayRateFiltered(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..."},null);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..."},null);
            return View(new GenelFilteredModel() { StudentId =student.StudentId});
        }
        [HttpPost]
        public IActionResult PayRateFiltered(GenelFilteredModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            if (model.StartDate > model.EndDate)
            {
                ModelState.AddModelError("","Başlangıç Tarihi Bitiş Tarihinden Büyük Olamaz...");
                return View(model);
            }
            return RedirectToAction("PayRateFilteredPdf", "Students", new
            {
                StudentId = model.StudentId,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            } );
        }
        public async Task<IActionResult> PayRateFilteredPdf(GenelFilteredModel model)
        {
            if (!ModelState.IsValid)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası"},null);
			var paymentInfos = await _paymentInfoService.FindByIdsAsync(model.StudentId);
			List<ResultPaymentPdfModel> paymentModels = new();
			foreach (var item in paymentInfos)
			{
				var payment = await _paymentService.FindByIdsAsync(item.RateId, item.StudentId, item.RateStudentId);
				var rateStudent = await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).ThenInclude(r => r.Branch).Where(r => r.StudentId == item.StudentId && r.RateId == item.RateId && item.RateStudentId == r.RateStudentId).FirstOrDefaultAsync();
				var frontPayment = await _context.FrontPayments.Include(f => f.RateStudent).AsNoTracking().FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId && r.RateStudent.StartRateDate >= model.StartDate && r.RateStudent.StartRateDate <= model.EndDate);

                if (frontPayment != null)
                {

                    if (payment == null)
                    {
                        paymentModels.Add(new ResultPaymentPdfModel() { AgeGroupName = rateStudent.Rate.AgeGroup.Name, BranchName = rateStudent.Student.Branch.BranchName, FullName = rateStudent.Student.Name + " " + rateStudent.Student.Surname, RateName = rateStudent.Rate.RateName, TotalPrice = rateStudent.RatePrice, LastPrice = rateStudent.RatePrice, RemaningPrice = 0 });
                    }
                    else
                    {
                        if (payment.RemaningPaymentPrice != 0)
                        {
                            paymentModels.Add(new ResultPaymentPdfModel() { AgeGroupName = rateStudent.Rate.AgeGroup.Name, BranchName = rateStudent.Student.Branch.BranchName, FullName = rateStudent.Student.Name + " " + rateStudent.Student.Surname, RateName = rateStudent.Rate.RateName, TotalPrice = rateStudent.RatePrice, LastPrice = payment.Bakiye, RemaningPrice = payment.RemaningPaymentPrice });
                        }

                    }
                }
			}
			return new Rotativa.AspNetCore.ViewAsPdf(paymentModels);

		}
		public async Task<IActionResult> PayRate(string slug)
        {
            if (slug == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var student = await _studentService.FindBySlugAsync(slug);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var paymentInfos = await _paymentInfoService.FindByIdsAsync(student.StudentId);
            List<ResultPaymentModel> paymentModels = new();
            foreach (var item in paymentInfos)
            {
                var payment = await _paymentService.FindByIdsAsync(item.RateId, item.StudentId, item.RateStudentId);
                var rateStudent = await _context.RateStudents.Where(r => r.StudentId == item.StudentId && r.RateId == item.RateId && item.RateStudentId == r.RateStudentId).FirstOrDefaultAsync();
                var frontPayment = await _context.FrontPayments.AsNoTracking().FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId);

                if (payment == null)
                {
                    paymentModels.Add(new ResultPaymentModel() { RateName = item.Rate.RateName, RatePrice = rateStudent.RatePrice, RemaningPrice = 0, StudentId = item.StudentId, RateSlug = item.Rate.Slug, PaymentId = 0, RateStudentId = rateStudent.RateStudentId, State = frontPayment.State, Bakiye = rateStudent.RatePrice });
                }
                else
                {
                    if (payment.RemaningPaymentPrice != 0)
                    {
                        paymentModels.Add(new ResultPaymentModel()
                        {
                            RateName = item.Rate.RateName,
                            RatePrice = rateStudent.RatePrice,
                            RemaningPrice = payment.RemaningPaymentPrice,
                            StudentId = item.StudentId,
                            RateSlug = item.Rate.Slug,
                            PaymentId = payment.PaymentId,
                            Bakiye = payment.Bakiye,
                            RateStudentId = rateStudent.RateStudentId,
                            State = frontPayment.State
                        });
                    }

                }
            }
            if (paymentModels.Count == 0)
                return AddError(new Error() { AlertType = "success", Description = "Ödenecek Borcunuz Yoktur..." }, student.Slug);
            return View(paymentModels);
        }
        public async Task<IActionResult> PayRateRapor(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası...." }, null);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
			var paymentInfos = await _paymentInfoService.FindByIdsAsync(student.StudentId);
			List<ResultPaymentPdfModel> paymentModels = new();
			foreach (var item in paymentInfos)
			{
				var payment = await _paymentService.FindByIdsAsync(item.RateId, item.StudentId, item.RateStudentId);
				var rateStudent = await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).ThenInclude(r => r.Branch).Where(r => r.StudentId == item.StudentId && r.RateId == item.RateId && item.RateStudentId == r.RateStudentId).FirstOrDefaultAsync();
				var frontPayment = await _context.FrontPayments.AsNoTracking().FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId);

				if (payment == null)
				{
					paymentModels.Add(new ResultPaymentPdfModel() { AgeGroupName = rateStudent.Rate.AgeGroup.Name, BranchName = rateStudent.Student.Branch.BranchName, FullName = rateStudent.Student.Name + " " + rateStudent.Student.Surname, RateName = rateStudent.Rate.RateName, TotalPrice = rateStudent.RatePrice, LastPrice = rateStudent.RatePrice,RemaningPrice = 0 });
				}
				else
				{
					if (payment.RemaningPaymentPrice != 0)
					{
                        paymentModels.Add(new ResultPaymentPdfModel() { AgeGroupName = rateStudent.Rate.AgeGroup.Name, BranchName = rateStudent.Student.Branch.BranchName, FullName = rateStudent.Student.Name + " " + rateStudent.Student.Surname, RateName = rateStudent.Rate.RateName, TotalPrice = rateStudent.RatePrice, LastPrice = payment.Bakiye, RemaningPrice = payment.RemaningPaymentPrice });
                    }

				}
			}
			return new  Rotativa.AspNetCore.ViewAsPdf(paymentModels);
    
		}

        public async Task<IActionResult> PayRates(int? studentId, int? id, int? rateStudentId)
        {
            if (id == null || studentId == null || rateStudentId == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var student = await _studentService.GetByIdAsync((int)studentId);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var banks = await _bankService.GetAllAsync();
            if (banks.Count == 0)
                return AddError(new Error() { AlertType = "danger", Description = "Banka olmadan ödeme yapamazsınız..." }, null);
            var rateStudent = await _context.RateStudents.Include(r => r.Rate).FirstOrDefaultAsync(r => r.StudentId == studentId && r.RateStudentId == rateStudentId);
            var rate = await _context.Rates.AsNoTracking().FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId);
            if (rate == null)
                return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..." }, null);
            if (student.Age < 20)
            {
                ViewBag.Parents = await _parentService.GetAllFilteredAsync(p => p.StudentId == studentId);
                if (ViewBag.Parents.Count == 0)
                    return AddError(new Error() { AlertType = "danger", Description = "Ebeveyn Bulunamadı..." }, student.Slug);
            }
            else

                ViewBag.Parents = null;

            ViewBag.paymentTypes = await _paymentTypeService.GetAllAsync();
            ViewBag.Banks = banks;
            double price = rateStudent.RatePrice;
            if (id != 0)
            {
                var payment = await _paymentService.GetByIdAsync((int)id);
                price = payment.Bakiye;
            }
            return View(new CreatePaymentModel() { PaymentId = (int)id, StudentId = (int)studentId, RateId = rate.RateId, RateStudentId = rateStudent.RateStudentId, RatePrice = price });
        }
        [HttpPost]
        public async Task<IActionResult> PayRates(CreatePaymentModel model)
        {
            var student = await _studentService.GetByIdAsync(model
                .StudentId);
            ViewBag.paymentTypes = await _paymentTypeService.GetAllAsync();
            ViewBag.Banks = await _bankService.GetAllAsync();
            if (student.Age < 20)
            {
                ViewBag.Parents = await _parentService.GetAllFilteredAsync(p => p.StudentId == student.StudentId);
                if (ViewBag.Parents.Count == 0)
                    return AddError(new Error() { AlertType = "danger", Description = "Ebeveyn Bulunamadı..." }, student.Slug);
                if (model.ParentId == null)
                {
                    ModelState.AddModelError("", "Lütfen Ebeveyn Seçiniz...");
                    return View(model);
                }

            }
            else
                ViewBag.Parents = null;
            if (!ModelState.IsValid)
                return View(model);
            var rateStudent = await _rateStudentService.GetByIdAsync(model.RateStudentId);
            var paymentType = (await _paymentTypeService.GetAllAsync()).FirstOrDefault(b => b.PaymentTypeName.ToLower() == "nakit");
            var frontPayment = await _frontPaymentService.FindByIdAsync(model.RateId, rateStudent.RateStudentId);
            if (model.PaymentId == 0)
            {
                if (model.Price > rateStudent.RatePrice)
                {
                    ModelState.AddModelError("", "Borçtan Fazlasını ödeyemezsiniz...");
                    return View(model);
                }
                Payment _payment = new Payment()
				{
					LastPaymentPrice = model.Price,
					PaymentDate = DateTime.Now,
					ParentId = model.ParentId,
					PaymentPrice = rateStudent.RatePrice,
					PaymentTypeId = model.PaymentTypeId,
					RateStudentId = rateStudent.RateStudentId,
					StudentId = model.StudentId,
					RateId = model.RateId,
					RemaningPaymentPrice = rateStudent.RatePrice - (model.RatePrice - model.Price),
					Bakiye = model.RatePrice - model.Price
				};
                await _paymentService.AddAsync(_payment);
                var _history = new HistoryPayment()
                {
                    DatePrice = DateTime.Now,
                    PaymentTypeId = model.PaymentTypeId,
                    Price = model.Price,
                    RateId = model.RateId,
                    StudentId = model.StudentId,
                };

                if (paymentType.PaymentTypeId != model.PaymentTypeId)
                    _history.BankId = model.BankId;

                await _historyPaymentService.AddAsync(_history);

                if (frontPayment != null)
                {
                    frontPayment.RemaningPrice = frontPayment.RatePrice - (model.Price);
                    frontPayment.Bakiye = model.RatePrice - frontPayment.RemaningPrice;
                    frontPayment.LastDate = DateTime.Now;
                    _frontPaymentService.Update(frontPayment);
                }

                return AddError(new Error() { AlertType = "danger", Description = "Başarıyla Ödendi..." }, student.Slug);
            }
            var payment = await _paymentService.GetByIdAsync(model.PaymentId);
            if (payment == null)
                return AddError(new Error() { AlertType = "danger", Description = "Ödeme Bulunamadı..." }, student.Slug);
            if (model.Price > payment.Bakiye)
            {
                ModelState.AddModelError("", "Borçtan Fazlasını ödeyemezsiniz...");
                return View(model);

            }
            payment.LastPaymentPrice = model.Price;
            payment.RemaningPaymentPrice = payment.RemaningPaymentPrice + model.Price;
            payment.Bakiye = payment.PaymentPrice - payment.RemaningPaymentPrice;
            payment.PaymentDate = DateTime.Now;
            _paymentService.Update(payment);
            var history = new HistoryPayment()
            {
                DatePrice = DateTime.Now,
                PaymentTypeId = model.PaymentTypeId,
                Price = model.Price,
                RateId = model.RateId,
                StudentId = model.StudentId,
            };

            if (paymentType.PaymentTypeId != model.PaymentTypeId)
                history.BankId = model.BankId;

            await _historyPaymentService.AddAsync(history);
            if (frontPayment != null)
            {
                frontPayment.RemaningPrice = frontPayment.RatePrice - (model.Price);
                frontPayment.Bakiye = model.RatePrice - frontPayment.RemaningPrice;
                frontPayment.LastDate = DateTime.Now;
                _frontPaymentService.Update(frontPayment);
            }
            return AddError(new Error() { AlertType = "success", Description = "Başarıyla Ödendi..." }, student.Slug);


        }
        public async Task<IActionResult> ShowHistoryPayment(string slug)
        {
            if (slug == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası...." }, null);
            var student = await _studentService.FindBySlugAsync(slug);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var historyPayments = await _historyPaymentService.FindByIdAsync(student.StudentId);
            return View(historyPayments);
        }
        public async Task<IActionResult> HistoryPaymentRapor(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası...." }, null);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var historyPayments = await _context.HistoryPayments.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).Include(r=>r.PaymentType).Include(r => r.Bank).AsNoTracking().Where(r => r.StudentId == student.StudentId).ToListAsync();
            return new Rotativa.AspNetCore.ViewAsPdf(historyPayments);

        
        }
        public async Task<IActionResult> AddExam(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı.." }, null);
            var rates = await _context.Rates.AsNoTracking().Where(r => r.BranchId == user.BranchId).ToListAsync();
            if (rates.Count == 0)
                return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı.." }, student.Slug);
            var _rates = await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.Branch).Include(r => r.Exam).ThenInclude(r => r.ExamResult).AsNoTracking().Where(r => r.StudentId == id && !r.State && r.Exam == null).ToListAsync();
            if (_rates.Count == 0)
                return AddError(new Error() { AlertType = "danger", Description = "Uygun Kur Bulunamadı..." }, student.Slug);
            ViewBag.Rates = _rates;
            return View(new CreateStudentExam() { StudentId = student.StudentId });
        }
        [HttpPost]
        public async Task<IActionResult> AddExam(CreateStudentExam model)
        {

            var student = await _studentService.GetByIdAsync(model.StudentId);
            var _rates = await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.Branch).Include(r => r.Exam).ThenInclude(r => r.ExamResult).AsNoTracking().Where(r => r.StudentId == model.StudentId && !r.State).ToListAsync();
            ViewBag.Rates = _rates;
            if (_rates.Count == 0)
                return AddError(new Error() { AlertType = "danger", Description = "Uygun Kur Bulunamadı..." }, student.Slug);
            if (!ModelState.IsValid)
                return View(model);
            var rate = await _rateService.GetByIdAsync(model.RateId);
            var rateStudent = await _context.RateStudents.Include(r => r.Rate).AsNoTracking().FirstOrDefaultAsync(r => r.RateId == rate.RateId && !r.State && r.StudentId == student.StudentId);
            if (rateStudent == null)
                return AddError(new Error() { AlertType = "danger", Description = "Kur Öğrenci BULUNAMADI..." }, student.Slug);
            if (model.ExamDate < DateTime.Now)
            {
                ModelState.AddModelError("", "Sınav Tarih Bilgisi Hatalıdır...");
                return View(model);
            }

            var uploadsFolder = Path.Combine("wwwroot/", "Exams");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var imagePath = Guid.NewGuid().ToString() + "_" + model.ExamPath.FileName;

            var _filePath = Path.Combine(uploadsFolder, imagePath);


            using (var stream = new System.IO.FileStream(_filePath, FileMode.Create))
            {
                await model.ExamPath.CopyToAsync(stream);
            }
            await _context.Exams.AddAsync(new Exam() { ExamPath = imagePath, ExamDate = model.ExamDate, RateId = rate.RateId, RateStudentId = rateStudent.RateStudentId, SuccessNote = model.SuccessNote, StudentId = student.StudentId });
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "danger", Description = "Sınav Eklendi..." }, student.Slug);

        }
        public async Task<IActionResult> EntryNote(int id)
        {
            var exam = await _context.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == id);
            if (exam == null)
                return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, null);
            return View(new EntryNoteModel() { ExamId = exam.ExamId });

        }
        [HttpPost]
        public async Task<IActionResult> EntryNote(EntryNoteModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var exam = await _context.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == model.ExamId);
            if (exam == null)
                return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, null);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == exam.StudentId);
            var rateStudent = await _context.RateStudents.Include(r => r.Exam).FirstOrDefaultAsync(r => r.Exam.ExamId == exam.ExamId);
            rateStudent.SuccessState = true;
            rateStudent.GraduationState = exam.SuccessNote >= model.Note ? false : true;
            await _context.ExamResults.AddAsync(new ExamResult() { ExamId = model.ExamId, RateId = exam.RateId, StudentId = exam.StudentId, Note = model.Note });
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "success", Description = "Sınav Girildi..." }, student.Slug);

        }
        public async Task<IActionResult> ChangeRate(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var exam = await _context.Exams.Include(r => r.Student).AsNoTracking().FirstOrDefaultAsync(r => r.ExamId == id);
            if (exam == null)
                return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, null);
            var rateStudent = await _rateStudentService.FindByIdandSlugAsync(exam.RateId, exam.Student.Slug);
            if (rateStudent == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var ageGroup = await _context.AgeGroups.AsNoTracking().FirstOrDefaultAsync(a => a.AgeGroupId == rateStudent.Rate.AgeGroupId);
            if (ageGroup == null)
                return AddError(new Error() { AlertType = "danger", Description = "Yaş Grubu Bulunamadı..." }, null);
            var rates = (await _rateService.FindByAgeandBranchAsync(ageGroup.AgeGroupId, rateStudent.Student.BranchId)).Where(r => r.RateId != exam.RateId).ToList();
            if (rates.Count == 0)
                return AddError(new Error() { AlertType = "danger", Description = "Başka Kur Olamadığı için kur değiştirme yapamazsınız..." }, null);
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
                return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..." }, null);
            var rateStudent = await _rateService.FindIncludeByIdAsync(model.RateStudentId);
            if (rateStudent == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            if (!(rateStudent.SuccessState && rateStudent.GraduationState))
                return AddError(new Error() { AlertType = "danger", Description = "İlk önce kuru tamamlamalısınız..." }, null);
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
            await _context.RatePaymentInfos.AddAsync(new RatePaymentInfo() { RateId = rate.RateId, RateStudentId = _rateStudent.RateStudentId, StudentId = _rateStudent.Student.StudentId });
            await _frontPaymentService.AddAsync(new FrontPayment() { Bakiye = 0, RateId = rate.RateId, RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100), RateStudentId = _rateStudent.RateStudentId, RemaningPrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100) });
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "success", Description = "Kur Başarılı bir şekilde değiştirildi..." }, rateStudent.Student.Slug);
        }
        public async Task<IActionResult> RepeatRate(int id)
        {
            var exam = await _context.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == id);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == exam.StudentId);
            var rate = await _context.Rates.AsNoTracking().FirstOrDefaultAsync(r => r.RateId == exam.RateId);
            if (rate == null)
                return AddError(new Error() { AlertType = "danger", Description = "Kur Bulunamadı..." }, null);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            return View(new RepeatRateModel() { RateId = rate.RateId, StudentId = student.StudentId });
        }
        [HttpPost]
        public async Task<IActionResult> RepeatRate(RepeatRateModel model)
        {
            if (!ModelState.IsValid)
                return View();
            var rate = await _rateService.GetByIdAsync(model.RateId);
            if (model.DiscountId == 0)
            {
                ModelState.AddModelError("", "Lütfen İndirim Seçiniz...");
                return View(model);
            }
            var discount = await _context.DiscountRates.FindAsync(model.DiscountId);
            var rateStudent = await _context.RateStudents.Include(r => r.Student).FirstOrDefaultAsync(r => r.StudentId == model.StudentId && r.RateId == model.RateId && !r.State);
            if (rateStudent == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            rateStudent.State = true;
            _context.RateStudents.Update(rateStudent);
            await _context.SaveChangesAsync();
            await _rateStudentService.AddAsync(new RateStudent() { RateId = model.RateId, StudentId = model.StudentId, RegisterDate = DateTime.Now, StartRateDate = model.StartRateDate, EndRateDate = model.StartRateDate.AddDays(TimeSpan.FromDays((rate.RateDate / 3 + 1) * 7).TotalDays), RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100) });
            var _rateStudent = await _context.RateStudents.OrderBy(r => r.RateStudentId).AsNoTracking().LastOrDefaultAsync();
            await _context.RatePaymentInfos.AddAsync(new RatePaymentInfo() { RateStudentId = _rateStudent.RateStudentId, RateId = _rateStudent.RateId, StudentId = _rateStudent.StudentId });
            await _context.FrontPayments.AddAsync(new FrontPayment()
            {
                RateId = rate.RateId,
                RemaningPrice = 0,
                Bakiye = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100),
                RatePrice = rate.RatePrice - (rate.RatePrice * discount.DiscountRates / 100),
                RateStudentId = _rateStudent.RateStudentId
            });
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "success", Description = "Kur Tekrarlama Eklendi..." }, rateStudent.Student.Slug);
        }
        public async Task<IActionResult> UpdateNote(int? id)
        {
            if (id == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası" }, null);
            var exam = await _context.Exams.Include(e => e.ExamResult).AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == id);
            if (exam == null)
                return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı" }, null);
            return View(new UpdateNoteModel() { ExamId = exam.ExamId, Note = exam.ExamResult.Note });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNote(UpdateNoteModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var exam = await _context.Exams.Include(e => e.Student).Include(e => e.ExamResult).AsNoTracking().FirstOrDefaultAsync(e => e.ExamId == model.ExamId);
            if (exam == null)
                return AddError(new Error() { AlertType = "danger", Description = "Sınav Bulunamadı..." }, null);
            if (exam.SuccessNote > model.Note)
            {
                var frontPayment = await _context.FrontPayments.FirstOrDefaultAsync(f => f.RateId == exam.RateId && f.RateStudentId == exam.RateStudentId);
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.RateStudentId == frontPayment.RateStudentId && p.RateId == frontPayment.RateId && !p.State);
                //if (payment != null)
                //{
                //    payment.State = true;
                //    _context.Payments.Update(payment);
                //    await _context.SaveChangesAsync();
                //}
                var rateStudent = await _context.RateStudents.FirstOrDefaultAsync(r => r.RateStudentId == exam.RateStudentId);
                rateStudent.State = false;
                rateStudent.GraduationState = false;
                await _context.SaveChangesAsync();
                //if (frontPayment != null)
                //{
                //    frontPayment.State = true;
                //    await _context.SaveChangesAsync();
                //}
            }
            else
            {
                var rateStundent = await _context.RateStudents.FirstOrDefaultAsync(r => exam.RateStudentId == r.RateStudentId);
                rateStundent.GraduationState = true;
                rateStundent.SuccessState = true;
            }
            exam.ExamResult.Note = model.Note;
            _context.ExamResults.Update(exam.ExamResult);
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "success", Description = "Not Güncelllendi" }, exam.Student.Slug);
        }
        public async Task<IActionResult> DeletePayment(int? id, int? studentId, int? rateStudentId)
        {
            if (id == null || studentId == null || rateStudentId == null)
                return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." }, null);
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
                return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." }, null);
            var rateStudent = await _context.RateStudents.FirstOrDefaultAsync(r => r.RateStudentId == rateStudentId);
            if (rateStudent == null)
                return AddError(new Error() { AlertType = "danger", Description = "Kur Öğrenci Bulunamadı..." }, null);
            var ratePaymentInfo = await _context.RatePaymentInfos.FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId);
            var frontPayment = await _context.FrontPayments.FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId && r.State);
            _context.FrontPayments.Remove(frontPayment);
            await _context.SaveChangesAsync();
            _context.RatePaymentInfos.Remove(ratePaymentInfo);
            await _context.SaveChangesAsync();
            if (payment != null)
                _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return AddError(new Error() { AlertType = "success", Description = "Silindi..." }, student.Slug);


        }
		public async Task<IActionResult> SendMailVeli(int id)
		{
			var student = await _context.Students.Include(s => s.Parents).AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == id);
			return View(student);
		}
		[HttpPost]
		public async Task<IActionResult> SendMailVeli(int studentId,int? id, IFormFile? file)
		{
			if (id == null)
			{
				ModelState.AddModelError("", "Mail Boş Geçilemez...");
				return View();
			}
			if (file == null)
			{
				ModelState.AddModelError("", "Lütfen  Bir Dosya Seçiniz...");
				return View();
			}
			var name = Guid.NewGuid().ToString();
			var path = Path.Combine("wwwroot/Mails/" + name + file.FileName);
			using (var stream = new FileStream(path, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			if (id == 0)
            {
                var parents = await _context.Parents.Where(s => s.StudentId == studentId).AsNoTracking().ToListAsync();
                foreach (var parent in parents)
                {
					MessageService.SendMail _mail = new MessageService.SendMail(parent.Mail, "Dosya Ektedir...", "Admin Tarafından Gönderildi...");
					await _mail.SendMailAsync(path);
				}
				return AddError(new Error() { AlertType = "success", Description = "Mail Gönderildi..." }, null);
			}
            var _parent = await _context.Parents.AsNoTracking().FirstOrDefaultAsync(p => p.ParentId == id);
			MessageService.SendMail mail = new MessageService.SendMail(_parent.Mail, "Dosya Ektedir...", "Admin Tarafından Gönderildi...");
			await mail.SendMailAsync(path);

			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);
			return AddError(new Error() { AlertType = "success", Description = "Mail Gönderildi..." },null);

		}
	}
}





