using AKUWebUI.Models;
using AKUWebUI.Views.Shared;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Rotativa;
using Rotativa.AspNetCore;
using SelectPdf;
using System.Reflection.Metadata;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles = $"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
	public class RaporsController : Controller
	{

		private readonly WebContext _context;
		private readonly UserManager<AppUser> _userManager;
		private List<Error> errors = new();

		public RaporsController(WebContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			return View(user);
		}
		public IActionResult SendMail()
		{
			return View();
		}
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Index","Rapors");
		}
		public async Task<IActionResult> SendMailVeli()
		{
			var students = await _context.Students.Include(s => s.Parents).AsNoTracking().ToListAsync();
			return View(students);
		}
		[HttpPost]
		public async Task<IActionResult> SendMailVeli(string Mail, IFormFile? file)
		{
			if (Mail == null)
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
			MessageService.SendMail mail = new MessageService.SendMail(Mail, "Dosya Ektedir...", "Admin Tarafından Gönderildi...");
			await mail.SendMailAsync(path);
			return AddError(new Error() { AlertType = "success", Description = "Mail Gönderildi..." });

		}

		[HttpPost]
		public async Task<IActionResult> SendMail(string Mail, IFormFile? file)
		{
			if (Mail == null)
			{
				ModelState.AddModelError("","Mail Boş Geçilemez...");
				return View();
			}
			if (file == null)
			{
				ModelState.AddModelError("","Lütfen  Bir Dosya Seçiniz...");
				return View();
			}
			var name = Guid.NewGuid().ToString();
			var path = Path.Combine("wwwroot/Mails/"+ name+file.FileName);
			using (var stream = new FileStream(path,FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			MessageService.SendMail mail = new MessageService.SendMail(Mail,"Dosya Ektedir...","Admin Tarafından Gönderildi...");
			await mail.SendMailAsync(path);

			if (System.IO.File.Exists(path))
				System.IO.File.Delete(path);
			return AddError(new Error() { AlertType ="success", Description ="Mail Gönderildi..."});
				
		}
		public async Task<IActionResult> AgeGroupsStudents()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.AgeGroups = user.Role == Rol.SuperAdmin ? await _context.AgeGroups.Include(r => r.Rates).ThenInclude(r => r.Branch).AsNoTracking().ToListAsync() : await _context.AgeGroups.Include(r => r.Rates).ThenInclude(r => r.Branch).AsNoTracking().ToListAsync();
			;
			ViewBag.BranchId = user.Role == Rol.SuperAdmin ? 0 : user.BranchId;
			return View();
			
		}
		[HttpPost]
		public async Task<IActionResult> AgeGroupsStudents(int? RateId)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.AgeGroups = await _context.AgeGroups.Include(r => r.Rates).ThenInclude(r => r.Branch).AsNoTracking().ToListAsync();
			ViewBag.BranchId = user.Role == Rol.SuperAdmin ? 0 : user.BranchId;
			if (RateId == null)
			{
				ModelState.AddModelError("", "Lütfen bir şube seçiniz...");
				return View();
			}
				return RedirectToAction("AgeGroupsStudentsPdf", "Rapors", new
				{
					RateId = RateId
				});
        }
		public async Task<IActionResult> AgeGroupsStudentsPdf(int RateId)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var rateStundents = RateId == 0 ?  (user.Role == Rol.SuperAdmin ? await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).ThenInclude(r => r.Branch).Where(r => r.Student.State && !r.State).AsNoTracking().ToListAsync() : await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).ThenInclude(r => r.Branch).Where(r => r.Student.State && !r.State && r.Student.BranchId == user.BranchId).AsNoTracking().ToListAsync()) : await _context.RateStudents.Include(r => r.Rate).ThenInclude(r => r.AgeGroup).Include(r => r.Student).ThenInclude(r => r.Branch).Where(r => r.RateId == RateId && r.Student.State && !r.State).AsNoTracking().ToListAsync();
			return new Rotativa.AspNetCore.ViewAsPdf(rateStundents);
		}
		public async Task<IActionResult> ActiveStudents()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			if(user.Role == Rol.SuperAdmin)
				return View(branches);
			return RedirectToAction("ActiveStudentsPdf", "Rapors", new
			{
				BranchId = user.BranchId
			});

		}
		[HttpPost]
		public async Task<IActionResult> ActiveStudents(int? BranchId)
		{
			if (BranchId == null)
			{
				ModelState.AddModelError("","Lütfen Bir Şube Seçiniz...");
				var branches = await _context.Branches.AsNoTracking().ToListAsync();
				return View(branches);
			}
				return RedirectToAction("ActiveStudentsPdf", "Rapors", new
				{
					branchId = BranchId
				} );
		}
		public async Task<IActionResult> ActiveStudentsPdf(int BranchId)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var students = user.Role == Rol.SuperAdmin ?(BranchId == 0 ? await _context.Students.Where(s => s.State).AsNoTracking().ToListAsync() : await _context.Students.Where(s => s.State && s.BranchId == BranchId).AsNoTracking().ToListAsync()) : await _context.Students.Where(s => s.State && s.BranchId == user.BranchId).AsNoTracking().ToListAsync();
			return new Rotativa.AspNetCore.ViewAsPdf(students);
		}
		public async Task<IActionResult> ShowBalanceFiltered()
		{

			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			ViewBag.Role = user.Role;
			if (user.Role == Rol.SuperAdmin)
				return View(branches);
			ViewBag.BranchId = user.BranchId;
			return View(branches);
			
		}
		[HttpPost]
		public async Task<IActionResult> ShowBalanceFiltered(int? BranchId,DateTime StartDate,DateTime EndDate)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			ViewBag.BranchId = user.BranchId;
			ViewBag.Role = user.Role;
			if (BranchId == null)
			{
				ModelState.AddModelError("", "Lütfen Şube Seçiniz...");
				return View(branches);
			}
			if (StartDate.Date > EndDate)
			{
				ModelState.AddModelError("","Başlangıç Tarihi Bitiş Tarihinden Büyük Olamaz...");
				return View(branches);
			}
			return RedirectToAction("ShowBalanceFilteredPdf", "Rapors", new
			{
				BranchId = BranchId,
				StartDate = StartDate,
				EndDate = EndDate
			});

		}
		public async Task<IActionResult> ShowBalanceFilteredPdf(int BranchId, DateTime StartDate, DateTime EndDate)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var payments = user.Role == Rol.SuperAdmin ? (BranchId == 0 ? await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0).AsNoTracking().ToListAsync() : await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0 && p.Student.BranchId == BranchId && p.PaymentDate >=StartDate && p.PaymentDate <=EndDate).AsNoTracking().ToListAsync()) : await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0 && p.Student.BranchId == user.BranchId && p.PaymentDate >= StartDate && p.PaymentDate <= EndDate).AsNoTracking().ToListAsync();
			var frontPayments = user.Role == Rol.SuperAdmin ? (BranchId == 0 ? await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f => f.RemaningPrice == 0 && f.RateStudent.StartRateDate >= StartDate && f.RateStudent.StartRateDate <= EndDate).ToListAsync() : await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f => f.Rate.BranchId == BranchId && f.RemaningPrice == 0 && f.RateStudent.StartRateDate >= StartDate && f.RateStudent.StartRateDate <= EndDate).ToListAsync()) : await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f => f.Rate.BranchId == BranchId && f.RemaningPrice == 0 && f.RateStudent.StartRateDate >= StartDate && f.RateStudent.StartRateDate <= EndDate).ToListAsync();
			foreach (var item in frontPayments)
			{
				payments.Add(new Payment() { Rate = item.Rate, Student = item.RateStudent.Student, Bakiye = item.Bakiye, RemaningPaymentPrice = item.RatePrice, PaymentPrice = item.RatePrice });
			}
			return new Rotativa.AspNetCore.ViewAsPdf(payments);
		}
		public async Task<IActionResult> ShowBalance()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			if (user.Role == Rol.SuperAdmin)
				return View(branches);
			return RedirectToAction("ShowBalancePdf", "Rapors", new
			{
				BranchId = user.BranchId
			} );
		}
		[HttpPost]
		public async Task<IActionResult> ShowBalance(int? BranchId)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			if (BranchId == null)
			{
				ModelState.AddModelError("","Şube Boş Bırakılamaz...");
				return View(branches);
			}
			return RedirectToAction("ShowBalancePdf", "Rapors", new
			{
				BranchId = BranchId
			});
		}
		public async Task<IActionResult> ShowBalancePdf(int? BranchId)
		{
		
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var payments = user.Role == Rol.SuperAdmin ? (BranchId == 0 ? await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0).AsNoTracking().ToListAsync() : await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0 && p.Student.BranchId == BranchId).AsNoTracking().ToListAsync()) : await _context.Payments.Include(p => p.Student).ThenInclude(r => r.Branch).Include(p => p.Rate).ThenInclude(p => p.AgeGroup).Where(p => p.Bakiye != 0 && p.Student.BranchId == user.BranchId).AsNoTracking().ToListAsync();
			var frontPayments = user.Role == Rol.SuperAdmin?(BranchId == 0 ? await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f =>  f.RemaningPrice == 0).ToListAsync() : await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f => f.Rate.BranchId == BranchId && f.RemaningPrice == 0).ToListAsync()) : await _context.FrontPayments.Include(f => f.Rate).Include(f => f.RateStudent).ThenInclude(f => f.Student).Where(f => f.Rate.BranchId == BranchId && f.RemaningPrice == 0).ToListAsync();
            foreach (var item in frontPayments)
            {
				payments.Add(new Payment() { Rate = item.Rate,Student = item.RateStudent.Student, Bakiye = item.Bakiye, RemaningPaymentPrice = item.RatePrice, PaymentPrice = item.RatePrice});
            }
            return new Rotativa.AspNetCore.ViewAsPdf(payments);
		}
		public async Task<IActionResult> ActiveRateStudents()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
			if(user.Role == Rol.SuperAdmin)
				return View();
			return RedirectToAction("ActiveRateStudentsPdf", new
			{
				BranchId = user.BranchId
			});
		}
		[HttpPost]
		public async Task<ActionResult> ActiveRateStudents(int? BranchId)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user.Role != Rol.SuperAdmin)
				return Redirect("/Rapors");
            ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
			if (BranchId == null)
			{
				ModelState.AddModelError("","Lütfen bir şube seçiniz...");
				return View();
			}
			return RedirectToAction("ActiveRateStudentsPdf", new
			{
				BranchId = BranchId 
			});
		}
		public async Task<IActionResult> ActiveRateStudentsPdf(int BranchId)
		{
			var students = BranchId == 0 ? await _context.RateStudents.Include(r => r.Student).ThenInclude(r => r.Branch).Include(r => r.Rate).Where(r => r.Student.State && !r.State).AsNoTracking().ToListAsync() : await _context.RateStudents.Include(r => r.Student).ThenInclude(r => r.Branch).Include(r => r.Rate).Where(r => r.Student.State && r.Student.BranchId == BranchId && !r.State).AsNoTracking().ToListAsync();
			return new Rotativa.AspNetCore.ViewAsPdf(students);
		}

	}
}
