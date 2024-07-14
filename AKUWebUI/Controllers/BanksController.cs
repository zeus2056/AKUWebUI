using AKUWebUI.Models.Bank;
using AKUWebUI.Models.Filtered;
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
	public class BanksController : Controller
	{
		private readonly IEFCoreBankService _bankService;
		private readonly UserManager<AppUser> _userManager;
		private readonly WebContext _context;
		private readonly List<Error> errors = new List<Error>();

		public BanksController(IEFCoreBankService bankService, UserManager<AppUser> userManager, WebContext context)
		{
			_bankService = bankService;
			_userManager = userManager;
			_context = context;
		}
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("/Banks");
		}
		public async Task<IActionResult> ShowAllBanksRapor(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var bank = await _context.Banks.AsNoTracking().FirstOrDefaultAsync(b => b.Slug == slug);
			if (bank == null)
				return AddError(new Error() { AlertType = "danger", Description = "Banka Bulunamadı..." });
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user.Role == Rol.SuperAdmin)
			{
				ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
				return View(bank.BankId);
			}
			return RedirectToAction("ShowAllBanksRaporPdf", "Banks", new
			{
				BranchId = user.BranchId,
				BankId = bank.BankId
			});
		}
		[HttpPost]
		public async Task<IActionResult> ShowAllBanksRapor(int? BranchId, int? BankId)
		{
			if (BranchId == null || BankId == null)
				return AddError(new Error() { AlertType = " danger", Description = "Banka Bulunamadı..." });
			if (BranchId != 0)
			{

				var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.BranchId == BranchId);
				if (branch == null)
					return AddError(new Error() { AlertType = "danger", Description = "Şube Bulunamadı..." });
			}
			if (BranchId == -1)
			{
				ModelState.AddModelError("", "Lütfen Bir Şube Seçiniz....");
				return View();
			}
			return RedirectToAction("ShowAllBanksRaporPdf", "Banks", new
			{
				BranchId = BranchId,
				BankId = BankId
			});
		}
		public async Task<IActionResult> ShowAllBanksRaporPdf(int? BranchId, int? BankId)
		{
			if (BranchId == null || BankId == null)
				return AddError(new Error() { AlertType = " danger", Description = "Banka Bulunamadı..." });
			if (BranchId != 0)
			{

				var branch = await _context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.BranchId == BranchId);
				if (branch == null)
					return AddError(new Error() { AlertType = "danger", Description = "Şube Bulunamadı..." });
			}
			return new Rotativa.AspNetCore.ViewAsPdf(BranchId == 0 ? await _context.HistoryPayments.Include(p => p.Rate).Include(p => p.Bank).Include(p => p.Student).Where(p => p.BankId == BankId).AsNoTracking().ToListAsync() : await _context.HistoryPayments.Include(p => p.Rate).Include(p => p.Bank).Include(p => p.Student).Where(p => p.BankId == BankId && p.Rate.BranchId == BranchId).AsNoTracking().ToListAsync());
		}
		public async Task<IActionResult> ShowFilteredBanksRapor(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var bank = await _context.Banks.AsNoTracking().FirstOrDefaultAsync(b => b.Slug == slug);
			if (bank == null)
				return AddError(new Error() { AlertType = "danger", Description = "Banka Bulunamadı..." });
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = user.Role;
			ViewBag.Branches = await _context.Branches.AsNoTracking().ToListAsync();
			return View(new FilteredBankModel() { BankId = bank.BankId });

		}
		[HttpPost]
		public async Task<IActionResult> ShowFilteredBanksRapor(FilteredBankModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user.Role == Rol.SuperAdmin)
			{
				if (model.BranchId == -1)
					return AddError(new Error() { AlertType = "danger", Description = "Lütfen Şube Seçiniz..." });
				if (model.StartDate.Date > model.EndDate.Date)
					return AddError(new Error() { Description = "Başlangıç Tarihi Bitiş Tarihinden Büyük Olamaz...", AlertType = "danger" });
				return RedirectToAction("ShowFilteredBanksRaporPdf", "Banks", new
				{
					BankId = model.BankId,
					BranchId = model.BranchId,
					StartDate = model.StartDate,
					EndDate = model.EndDate
				});
			}
			else
			{
				if (model.StartDate.Date > model.EndDate.Date)
					return AddError(new Error() { Description = "Başlangıç Tarihi Bitiş Tarihinden Büyük Olamaz...", AlertType = "danger" });
				return RedirectToAction("ShowFilteredBanksRaporPdf", "Banks", new
				{
					BankId = model.BankId,
					BranchId = user.BranchId,
					StartDate = model.StartDate,
					EndDate = model.EndDate
				});
			}
		}
		public async Task<IActionResult> ShowFilteredBanksRaporPdf(FilteredBankModel model)
		{
			return new Rotativa.AspNetCore.ViewAsPdf(model.BranchId == 0 ? await _context.HistoryPayments.Include(p => p.Rate).Include(p => p.Bank).Include(p => p.Student).Where(p => p.BankId == model.BankId && p.DatePrice.Date >= model.StartDate && p.DatePrice.Date <= model.EndDate).AsNoTracking().ToListAsync() : await _context.HistoryPayments.Include(p => p.Rate).Include(p => p.Bank).Include(p => p.Student).Where(p => p.BankId == model.BankId && p.Rate.BranchId == model.BranchId && p.DatePrice.Date >= model.StartDate && p.DatePrice.Date <= model.EndDate).AsNoTracking().ToListAsync());
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher;
			var banks = await _bankService.GetAllAsync();
			return View(banks);
		}
		public IActionResult CreateBank()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateBank(CreateBankModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var bank = await _bankService.GetAllFilteredAsync(b => b.BankName.ToLower() == model.BankName.ToLower());
			if (bank.Count > 0)
				return AddError(new Error() { AlertType = "danger", Description = "Banka ismi kullanılıyor" });
			await _bankService.AddAsync(new Bank() { BankName = model.BankName, Slug = model.BankName.Replace(" ", "-") });
			return AddError(new Error() { AlertType = "success", Description = "Banka eklendi..." });

		}
		public async Task<IActionResult> DeleteBank(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre hatası var..." });
			var bank = await _bankService.FindBySlugAsync(slug);
			if (bank == null)
				return AddError(new Error() { AlertType = "danger", Description = "Banka bulunamadı..." });
			_bankService.Delete(bank);
			return AddError(new Error() { AlertType = "danger", Description = "Banka silindi..." });
		}
		public async Task<IActionResult> UpdateBank(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre hatası var..." });
			var bank = await _bankService.FindBySlugAsync(slug);
			if (bank == null)
				return AddError(new Error() { AlertType = "danger", Description = "Banka bulunamadı..." });
			return View(new UpdateBankModel() { BankId = bank.BankId, BankName = bank.BankName });
		}
		[HttpPost]
		public async Task<IActionResult> UpdateBank(UpdateBankModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var bankValidate = await _bankService.GetAllFilteredAsync(b => b.BankId != model.BankId && b.BankName == model.BankName);
			if (bankValidate.Count > 0)
				return AddError(new Error() { AlertType = "danger", Description = "Banka ismi zaten kullanılıyor..." });
			var bank = await _bankService.GetByIdAsync(model.BankId);
			if (bank == null)
				return AddError(new Error() { AlertType = "danger", Description = "Banka bulunamadı..." });
			bank.BankName = model.BankName;
			bank.Slug = model.BankName.Replace(" ", "-");
			_bankService.Update(bank);
			return AddError(new Error() { AlertType = "success", Description = "Banka güncellendi..." });
		}
	}
}
