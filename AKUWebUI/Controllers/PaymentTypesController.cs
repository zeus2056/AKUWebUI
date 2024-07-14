using AKUWebUI.Models.PaymentType;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	[Authorize]
	public class PaymentTypesController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCorePaymentTypeService _paymentTypeService;
		private List<Error> errors = new();

		public PaymentTypesController(UserManager<AppUser> userManager, IEFCorePaymentTypeService paymentTypeService)
		{
			_userManager = userManager;
			_paymentTypeService = paymentTypeService;
		}

		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("~/PaymentTypes");
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher;
			var paymentTyes = await _paymentTypeService.GetAllAsync();
			return View(paymentTyes);
		}
		public IActionResult CreatePaymentType()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreatePaymentType(CreatePaymentTypeModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var paymentTypeValidate = await _paymentTypeService.GetAllFilteredAsync(p => p.PaymentTypeName.ToLower() == model.PaymentTypeName.ToLower());
			if (paymentTypeValidate.Count > 0)
			{
				ModelState.AddModelError("","Ödeme tipi zaten var...");
				return View(model);
			}
			await _paymentTypeService.AddAsync(new PaymentType()
			{
				PaymentTypeName = model.PaymentTypeName
			,
				Slug = model.PaymentTypeName.Replace(" ", "-")
			});
			return AddError(new Error() { AlertType="success",Description = "Ödeme Tipi Eklendi..."});
		}
		public async Task<IActionResult> DeletePaymentType(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger",Description = "Parametre hatası..."});
			var paymentType = await _paymentTypeService.FindBySlugAsync(slug);
			if (paymentType == null)
				return AddError(new Error() { AlertType = "warning",Description = "Ödeme Tipi Bulunamadı..."});
			_paymentTypeService.Delete(paymentType);
			return AddError(new Error() { AlertType = "success",Description = "Ödeme Tipi Silindi..."});
		}
		public async Task<IActionResult> UpdatePaymentType(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre hatası..." });
			var paymentType = await _paymentTypeService.FindBySlugAsync(slug);
			if (paymentType == null)
				return AddError(new Error() { AlertType = "warning", Description = "Ödeme Tipi Bulunamadı..." });
			return View(new UpdatePaymentTypeModel() { PaymentTypeId = paymentType.PaymentTypeId, PaymentTypeName = paymentType.PaymentTypeName});
		}
		[HttpPost]
		public async Task<IActionResult> UpdatePaymentType(UpdatePaymentTypeModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var paymentTypeValidate = (await _paymentTypeService.GetAllFilteredAsync(p => p.PaymentTypeId != model.PaymentTypeId && p.PaymentTypeName.ToLower() == model.PaymentTypeName.ToLower()));
			if (paymentTypeValidate.Count > 0)
			{
				ModelState.AddModelError("","Ödeme tipi kullanılıyor...");
				return View(model);
			}
			var paymentType = await _paymentTypeService.GetByIdAsync(model.PaymentTypeId);
			if (paymentType == null)
				return AddError(new Error() { AlertType = "danger",Description = "Ödeme tipi bulunamadı..." });
			paymentType.PaymentTypeName = model.PaymentTypeName;
			paymentType.Slug = model.PaymentTypeName.Replace(" ", "-");
			_paymentTypeService.Update(paymentType);
			return AddError(new Error() { AlertType = "success", Description = "Ödeme Tipi Güncellendi..." });
		}
	}
}
