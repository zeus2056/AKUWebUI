using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AKUWebUI.ViewComponents
{
	public class ParentInfoComponent : ViewComponent
	{
		private readonly IEFCoreParentService _parentService;
		private readonly UserManager<AppUser> _userManager;

		public ParentInfoComponent(IEFCoreParentService parentService,UserManager<AppUser> userManager)
		{
			_userManager = userManager;
			_parentService = parentService;
		}

		public async Task<IViewComponentResult> InvokeAsync(int id)
		{
			ViewBag.Role = (await _userManager.FindByNameAsync(User.Identity.Name)).Role < Rol.Teacher ? true : false;
			var parents = await _parentService.GetAllFilteredAsync(p => p.StudentId == id);
			return View(parents);
		}
	}
}
