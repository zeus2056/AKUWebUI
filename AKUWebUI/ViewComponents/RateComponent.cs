using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI.ViewComponents
{
	public class RateComponent : ViewComponent
	{
		private readonly WebContext _context;
		private readonly UserManager<AppUser> _userManager;

		public RateComponent(WebContext context,UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			var rates = user.Role == Rol.SuperAdmin ? await _context.Rates.Where(r => r.RateState).ToListAsync() : 
				await _context.Rates.Where(r => r.RateState && r.BranchId == user.BranchId).ToListAsync();
			bool State = true;
			var _rates = new List<Rate>();
			foreach (var rate in rates)
			{
				foreach (var _rate in _rates)
				{
					if (rate.RateName == _rate.RateName)
					{
						State = false;
						break;
					}
				}
				if (State)
					_rates.Add(rate);
				State = true;
			}
			return View(_rates);
		}
	}
}
