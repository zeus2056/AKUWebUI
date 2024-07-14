using EntityLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AKUWebUI.ViewComponents
{
    public class NavbarComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public NavbarComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userRole = (await _userManager.GetRolesAsync(user))[0];
            ViewBag.UserRole = true;
            ViewBag.Gender = user.Gender ? true : false;
            if (userRole == Rol.Teacher.ToString())
               ViewBag.UserRole = false;
            return View();
        }
    }
}
