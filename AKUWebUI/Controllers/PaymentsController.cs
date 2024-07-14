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
	public class PaymentsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCorePaymentService _paymentService;
		private readonly IEFCoreFrontPaymentService _frontPaymentService;
		private readonly WebContext _context;
		private List<Error> errors = new();
		[NonAction]
		private IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("/Payments");
		}

        public PaymentsController(UserManager<AppUser> userManager, IEFCorePaymentService paymentService,IEFCoreFrontPaymentService frontPaymentService,WebContext context)
        {
            _userManager = userManager;
            _paymentService = paymentService;
			_frontPaymentService = frontPaymentService;
			_context = context;
        }
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = user.Role;
			var payments = await _paymentService.GetAllPaymentAsync(user.Role == Rol.SuperAdmin ? 0 : user.BranchId);
			var frontPayments = await _frontPaymentService.GetAllFrontPaymentsAsync();
			var lists = new List<FrontPayment>();
			if (payments.Count == 0)
			{
				ViewBag.FrontPayments = frontPayments;
				return View(payments);
			}

			else
			{
				foreach (var item in frontPayments)
				{
					lists.Add(item);
					foreach (var fr in payments)
					{
						if (item.RateStudentId == fr.RateStudentId && item.RateId == fr.RateId)
							lists.Remove(item);
					}
				}
			}
			ViewBag.FrontPayments = lists;
			return View(payments);
		}
		public async Task<IActionResult> RemovePayment(int? id, int? studentId, int? rateStudentId)
		{
			if (id == null || studentId == null || rateStudentId == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var frontPayment = await _context.FrontPayments.FirstOrDefaultAsync(r => r.FrontPaymentId == id);
			var payment = await _context.Payments.FirstOrDefaultAsync(p => p.RateId == frontPayment.RateId && p.RateStudentId == frontPayment.RateStudentId && p.State );
			var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentId);
			if (student == null)
				return AddError(new Error() { AlertType = "danger", Description = "Öğrenci Bulunamadı..." });
			var rateStudent = await _context.RateStudents.FirstOrDefaultAsync(r => r.RateStudentId == rateStudentId);
			if (rateStudent == null)
				return AddError(new Error() { AlertType = "danger", Description = "Kur Öğrenci Bulunamadı..." });
			var ratePaymentInfo = await _context.RatePaymentInfos.FirstOrDefaultAsync(r => r.RateId == rateStudent.RateId && r.RateStudentId == rateStudent.RateStudentId);
			
			_context.FrontPayments.Remove(frontPayment);
			await _context.SaveChangesAsync();
			_context.RatePaymentInfos.Remove(ratePaymentInfo);
			await _context.SaveChangesAsync();
			if (payment != null)
				_context.Payments.Remove(payment);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "success", Description = "Silindi..." });

		}
		public async Task<IActionResult> DeletePayment(int? id)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user.Role != Rol.SuperAdmin)
				return Redirect("/Login/AccessDenied");
			if (id == null)
				return AddError(new Error() { Description= "Parametre Hatası", AlertType = "danger"});
			var payment = await _paymentService.GetByIdAsync((int)id);
			if (payment == null)
				return AddError(new Error() { AlertType = "danger", Description = "Ödeme Bulunamadı..."});
			_paymentService.Delete(payment);
			return AddError(new Error() { AlertType = "success", Description = "Ödeme Silindi..."});
		}
	}
}
