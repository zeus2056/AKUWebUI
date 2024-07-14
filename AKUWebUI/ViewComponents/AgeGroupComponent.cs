using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI.ViewComponents
{
	public class AgeGroupComponent : ViewComponent
	{
		private readonly WebContext _context;

		public AgeGroupComponent(WebContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync(int? id)
		{
			if (id != null)
				ViewBag.Selected = id;
			else
				ViewBag.Selected = 0;
			var ageGroups = await _context.AgeGroups.AsNoTracking().ToListAsync();
			return View(ageGroups);
		}
	}
}
