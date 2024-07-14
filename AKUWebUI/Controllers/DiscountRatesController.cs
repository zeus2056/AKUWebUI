using AKUWebUI.Models.DiscountRate;
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
	[Authorize(Roles = $"{nameof(Rol.SuperAdmin)}")]
	public class DiscountRatesController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreDiscountRateService _discountRateService;
		private List<Error> errors = new();
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("~/DiscountRates");
		}
		public DiscountRatesController(UserManager<AppUser> userManager, IEFCoreDiscountRateService discountRateService)
		{
			_userManager = userManager;
			_discountRateService = discountRateService;
		}
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Admin;
			var discountRates = await _discountRateService.GetAllAsync();
			return View(discountRates);
		}
		public IActionResult CreateDiscountRate()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateDiscountRate(CreateDiscountRateModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var validate = await _discountRateService.GetAllFilteredAsync(d => d.DiscontRateName.ToLower() == model.DiscountRateName.ToLower());
				if (validate.Count > 0)
				{
					ModelState.AddModelError("", "İndirim Oran Adı alınmış...");
					return View(model);
				}
			await _discountRateService.AddAsync(new DiscountRate() { DiscontRateName = model.DiscountRateName, DiscountRates = model.DiscountRate});
			return AddError(new Error() { AlertType = "success",Description = "İndirim Oranı Eklendi..."});
		}
		public async Task<IActionResult> DeleteDiscountRate(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "danger",Description = "Parametre Hatasi var..."});
			var discountRate = await _discountRateService.GetByIdAsync((int)id);
			if (discountRate == null)
				return AddError(new Error() { AlertType = "danger",Description="İndirim Oranı Bulunamadı..."});
			_discountRateService.Delete(discountRate);
			return AddError(new Error() { AlertType = "success",Description = "İndirim Oranı Silindi..."});
		}
		public async Task<IActionResult> UpdateDiscountRate(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatasi var..." });
			var discountRate = await _discountRateService.GetByIdAsync((int)id);
			if (discountRate == null)
				return AddError(new Error() { AlertType = "danger", Description = "İndirim Oranı Bulunamadı..." });
			return View(new UpdateDiscountRateModel() { DiscountRate = discountRate.DiscountRates, DiscountRateId = discountRate.DiscountRateId, DiscountRateName = discountRate.DiscontRateName});
		}
		[HttpPost]
		public async Task<IActionResult> UpdateDiscountRate(UpdateDiscountRateModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var discountRate = await _discountRateService.GetByIdAsync(model.DiscountRateId);
			if (discountRate == null)
				return AddError(new Error() { AlertType = "danger", Description = "İndirim Oranı Bulunamadı..." });
			var validate = await _discountRateService.GetAllFilteredAsync(d => d.DiscountRateId != model.DiscountRateId && (d.DiscontRateName.ToLower() == model.DiscountRateName.ToLower() || d.DiscountRates == model.DiscountRate));
			if (validate.Count > 0)
			{
				ModelState.AddModelError("", "İndirim Oran Adı veya indirim oranı alınmış...");
				return View(model);
			}
			
			discountRate.DiscontRateName = model.DiscountRateName;
			discountRate.DiscountRates = model.DiscountRate;
			_discountRateService.Update(discountRate);
			return AddError(new Error() { AlertType = "success",Description = "İndirim Oranı Güncellendi..."});
		}
	}
}
