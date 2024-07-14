using AKUWebUI.Models.Expens;
using AKUWebUI.Views.Shared;
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
	public class ExpensesController : Controller
	{
		private readonly WebContext _context;
		private List<Error> errors  = new();
		private readonly UserManager<AppUser> _userManager;

		public ExpensesController(WebContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Index","Expenses");
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var expenses = user.Role == Rol.SuperAdmin ? await _context.Expenses.Include(e => e.Branch).AsNoTracking().ToListAsync()
				 : await _context.Expenses.Include(e => e.Branch).Where(e => e.BranchId == user.BranchId).AsNoTracking().ToListAsync();
			return View(expenses);
		}
		public async Task<IActionResult> CreateExpens()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateExpens(CreateExpensModel model)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!ModelState.IsValid)
				return View(model);
			if (model.BranchId == 0)
			{
				ModelState.AddModelError("", "Lutfen Şube Seçiniz...");
				return View(model);
			}
			if (model.ExpensDate < DateTime.Now.Date)
			{
				ModelState.AddModelError("","Tarih Bilgisi Hatalıdır...");
				return View(model);
			}
			await _context.Expenses.AddAsync(new Expens() { BranchId = (int)(user.Role == Rol.SuperAdmin ? model.BranchId : user.BranchId),Description = model.Description, ExpensDate = model.ExpensDate, ExpensPrice = model.Price  });
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "success", Description = "Gider Eklendi..."});
		}
		public async Task<IActionResult> UpdateExpens(int? id)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			if (id == null)
				return AddError(new Error() { AlertType ="danger", Description = "Parametre Hatası..."});
			var expens = await _context.Expenses.Include(e => e.Branch).AsNoTracking().FirstOrDefaultAsync(e => e.ExpensId == id);
			if (expens == null)
				return AddError(new Error() { AlertType ="danger", Description = "Gider Bulunamadı..."});
			return View(new UpdateExpensModel() { BranchId = expens.BranchId , Description = expens.Description, ExpensDate = expens.ExpensDate , Price = expens.ExpensPrice, ExpensId = expens.ExpensId});
		}
		[HttpPost]
		public async Task<IActionResult> UpdateExpens(UpdateExpensModel model)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			if (!ModelState.IsValid)
				return View(model);
			if (model.BranchId == 0)
			{
				ModelState.AddModelError("","Lütfen şube seçiniz...");
				return View(model);
			}
			if (model.ExpensDate < DateTime.Now.Date)
			{
				ModelState.AddModelError("", "Tarih Bilgisi Hatalıdır...");
				return View(model);
			}
			var expens = await _context.Expenses.Include(e => e.Branch).FirstOrDefaultAsync(e => e.ExpensId == model.ExpensId);
			if (expens == null)
				return AddError(new Error() { AlertType = "danger", Description = "Gider Bulunamadı..."});
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			expens.ExpensDate = model.ExpensDate;
			expens.ExpensPrice = model.Price;
			expens.Description = model.Description;
			expens.BranchId = (int)(user.Role == Rol.SuperAdmin ? model.BranchId : expens.BranchId);
			_context.Expenses.Update(expens);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType ="success", Description = "Gider Güncellendi..."});

		}
		public async Task<IActionResult> DeleteExpens(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "danger", Description = "Gider Bulunamadı..."});
			var expens = await _context.Expenses.FirstOrDefaultAsync(e => e.ExpensId == id);
			if (expens == null)
				return AddError(new Error() { AlertType = "danger", Description = "Gider Bulunamadı..." });
			_context.Expenses.Remove(expens);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType="danger", Description = "Gider Silindi..."});

		}
	}
}
