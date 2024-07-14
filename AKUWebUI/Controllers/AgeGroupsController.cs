using AKUWebUI.Models.AgeGroup;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EntityLayer.Entities ;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles =$"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
	public class AgeGroupsController : Controller
	{
		private readonly IEFCoreAgeGroupService _ageGroupService;
		private readonly UserManager<AppUser> _userManager;
		private readonly WebContext _context;
		private List<Error> errors = new();

		public AgeGroupsController(IEFCoreAgeGroupService ageGroupService,UserManager<AppUser> userManager,WebContext context)
		{
			_ageGroupService = ageGroupService;
			_userManager = userManager;
			_context = context;
		}
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher ? true : false;
			var ageGroups = await _ageGroupService.GetAllAsync();
			return View(ageGroups);
		}
		[NonAction]
		public IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Index", "AgeGroups");
		}
		public IActionResult CreateAgeGroup()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateAgeGroup(CreateAgeGroupModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var ageGroups = await _ageGroupService.GetAllAsync();
			var validate = await _context.AgeGroups.Where(a => a.Name.Replace(" ", "-").ToLower() == model.Name.Replace(" ", "-").ToLower()).AsNoTracking().ToListAsync();
			if (validate.Count > 0)
			{
				ModelState.AddModelError("","Bu isimde zaten yaş grubu var...");
				return View(model);
			}

			if (model.EndAge < model.StartAge)
			{
				ModelState.AddModelError("","Başlangıç Yaşı Bitiş Yaşından Büyük Olamaz...");
				return View(model);
			}
				
			await _ageGroupService.AddAsync(new AgeGroup() { Name = model.Name,StartAge = model.StartAge,EndAge = model.EndAge});
			return AddError(new Error() { AlertType = "warning", Description = "AgeGroup has been created..." });
		}
		public async Task<IActionResult> DeleteAgeGroup(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "warning", Description = "Id is required..." });
			var ageGroup = await _ageGroupService.GetByIdAsync((int)id);
			if (ageGroup == null)
				return AddError(new Error() { AlertType = "warning", Description = "AgeGroup has been not founded..." });
			_ageGroupService.Delete(ageGroup);
			return AddError(new Error() { AlertType = "warning", Description = "AgeGroup has been deleted..." });
		}
		public async Task<IActionResult> EditAgeGroup(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType = "warning",Description = "Id is required..."});
            var ageGroup = await _ageGroupService.GetByIdAsync((int)id);
            if (ageGroup == null)
                return AddError(new Error() { AlertType = "warning", Description = "AgeGroup has been not founded..." });
			return View(new EditAgeGroupModel() { Id = ageGroup.AgeGroupId,Name = ageGroup.Name, EndAge = ageGroup.EndAge, StartAge = ageGroup.StartAge});
        }
		[HttpPost]
		public async Task<IActionResult> EditAgeGroup(EditAgeGroupModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var ageGroup = await _ageGroupService.GetByIdAsync((int)model.Id);
			if (ageGroup == null)
				return AddError(new Error() { AlertType = "warning", Description = "AgeGroup has been not founded..." });
			var validate =  await _context.AgeGroups.Where(a => a.AgeGroupId != model
			.Id && a.Name.Replace(" ", "-").ToLower() == model.Name.Replace(" ", "-").ToLower()).AsNoTracking().ToListAsync();
			if (validate.Count > 0)
			{
				ModelState
					.AddModelError("","Bu isimde zaten yaş grubu var...");
				return View(model);
			}
			if (model.EndAge < model.StartAge)
				return AddError(new Error() { AlertType = "danger", Description = "Başlangıç yaşı bitiş yaşından buyuk olamaz..." });
			ageGroup.StartAge = model.StartAge;
			ageGroup.EndAge = model.EndAge;
			ageGroup.Name = model.Name;
			_ageGroupService.Update(ageGroup);
			return AddError(new Error() { AlertType = "warning",Description = "AgeGroup has been updated..."});
		}
	}
}
