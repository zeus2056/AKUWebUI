using EntityLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKUWebUI.ViewComponents
{
    public class SideBarComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly WebContext _context;

        public SideBarComponent(UserManager<AppUser> userManager,WebContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _context.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var UserRole = (await _userManager.GetRolesAsync(user))[0];
            ViewBag.Slug = user.Slug;
            ViewBag.UserRole = UserRole;
            ViewBag.Gender = user.Gender;
            return View(user);
        }
    }
}
