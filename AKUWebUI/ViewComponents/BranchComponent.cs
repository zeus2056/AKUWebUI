using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI.ViewComponents
{
	public class BranchComponent : ViewComponent
	{
		private readonly WebContext _context;

		public BranchComponent(WebContext context)
		{
			_context = context;
		}
		public async Task<IViewComponentResult> InvokeAsync(int SelectedId)
		{
			ViewBag.SelectedId = SelectedId;
			var branches = await _context.Branches.AsNoTracking().ToListAsync();
			return View(branches);
		}
	}
}
