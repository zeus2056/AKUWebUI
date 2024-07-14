using AKUWebUI.Models.Permission;
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
	[Authorize(Roles =$"{nameof(Rol.SuperAdmin)}")]
	public class PermissionsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCorePermissionService _permissionService;
		private List<Error> errors = new();

		public PermissionsController(UserManager<AppUser> userManager, IEFCorePermissionService permissionService)
		{
			_userManager = userManager;
			_permissionService = permissionService;
		}
		private IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("/Permissions");
		}
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Admin;
			var permissions = await _permissionService.GetAllAsync();
			return View(permissions);
		}
		public IActionResult CreatePermission()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreatePermission(CreatePermissionModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var permission = await _permissionService.FindByNameAsync(model.Name);
			if (permission != null)
			{
				ModelState.AddModelError("", "\"İzin İsmi kullanılıyor...\" ");
				return View(model);
			}
			var permissionValidate = await _permissionService.GetAllFilteredAsync(p => p.YearCount == model.YearCount);
			if (permissionValidate.Count > 0)
			{
				ModelState.AddModelError("", "Yıl sayısına göre izin zaten var...");
				return View(model);
			}
			await _permissionService.AddAsync(new Permission() { Name = model.Name, DayCount = model.DayCount, YearCount = model.YearCount });
			return AddError(new Error() { AlertType= "success", Description = "İzin Eklendi..."});
		}
		public async Task<IActionResult> DeletePermission(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType ="danger", Description = "Parametre Hatası..."});
			var permission = await _permissionService.GetByIdAsync((int)id);
			if (permission == null)
				return AddError(new Error() { AlertType= "danger", Description = "İzin Bulunamadı..."});
			_permissionService.Delete(permission);
			return AddError(new Error() {AlertType = "danger", Description = "İzin Silindi..." });
		}
		public async Task<IActionResult> UpdatePermission(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var permission = await _permissionService.GetByIdAsync((int)id);
			if (permission == null)
				return AddError(new Error() { AlertType = "danger", Description = "İzin Bulunamadı..." });
			return View(new UpdatePermissionModel() { PermissionId = permission.PermissionId, DayCount = permission.DayCount, Name = permission.Name, YearCount= permission.YearCount});
		}
		[HttpPost]
		public async Task<IActionResult> UpdatePermission(UpdatePermissionModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var permission = await _permissionService.GetByIdAsync(model.PermissionId);
			if (permission == null)
				return AddError(new Error() { AlertType ="danger", Description = "İzin Bulunamadı..."});
			var validate = await _permissionService.GetAllFilteredAsync(p => p.PermissionId != model.PermissionId && (p.Name == model.Name || p.YearCount == model.YearCount));
			if (validate.Count > 0)
			{
				ModelState.AddModelError("", "İsim Yada Yıl Sayısı zaten kullanılıyor....");
				return View(model);
			}
			permission.Name = model.Name;
			permission.YearCount = model.YearCount;
			permission.DayCount = model.DayCount;
			_permissionService.Update(permission);
			return AddError(new Error() { AlertType = "success", Description = "İzin Güncellendi..."});
		}
	}
}
