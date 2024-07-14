using AKUWebUI.MessageService;
using AKUWebUI.Models.Login;
using AKUWebUI.Models.Register;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	public class LoginController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IEFCoreStaffService _staffService;
		private readonly RoleManager<AppRole> _roleManager;
		private readonly WebContext _context;

		public LoginController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,IEFCoreStaffService staffService,WebContext context,RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
			_roleManager = roleManager;	
			_staffService = staffService;
			DbSeed.Initialize(new WebContext());
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
		public  async Task<IActionResult> Index()
		{
			var user = await _context.Users.FirstOrDefaultAsync(U => U.Role == Rol.SuperAdmin);
			if (!_context.Roles.Any())
			{
				await _roleManager.CreateAsync(new AppRole() { Name = Rol.SuperAdmin.ToString()});
				await _roleManager.CreateAsync(new AppRole() { Name = Rol.Admin.ToString()});
				await _roleManager.CreateAsync(new AppRole() { Name = Rol.Teacher.ToString()});
			}
			if (user == null)
			{
                await _userManager.CreateAsync(new AppUser() { Name = "Naim", Surname = "Karasekreter", BranchId = 1, Date = DateTime.Now, Email = "herkez1646@gmail.com", Gender = true, Role = Rol.SuperAdmin, Slug = "Naim-Karasekreter", TC = "12345678912", UserName = "naim2056", EmailConfirmed = true }, "Veliengin1.");
                var _user = await _context.Users.OrderBy(u => u.Id).LastOrDefaultAsync();
				await _userManager.AddToRoleAsync(_user,Rol.SuperAdmin.ToString());
					
            }

            return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(LoginModel model)
		{
			if (User == null)
				return Redirect("/Admin");
			if (!ModelState.IsValid)
				return View(model);
			var emailValidate = await _userManager.FindByEmailAsync(model.Email);
			if (emailValidate == null)
			{
				ModelState.AddModelError("","Email has been not found");
				return View(model);
			}
			var emailConfirm = await _userManager.IsEmailConfirmedAsync(emailValidate);
			if (!emailConfirm)
			{
				ModelState.AddModelError("","Lütfen mailinizi onaylanıyınız...");
				return View(model);
			}
			var loginValidate = await _signInManager.PasswordSignInAsync(emailValidate, model.Password, false, false);
			if (loginValidate.Succeeded)
				return Redirect("/Admin");
			ModelState.AddModelError("","Email or Password doesn't correct");
			return View(model);
		}
		public IActionResult ForgetPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgetPassword(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				ModelState.AddModelError("","Email not found.");
				return View("ForgetPassword",email);
			}
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var url = Url.ActionLink("ResetPassword", "Login", new
			{
				Id = user.Id,
				ConfirmCode = token
			});
			SendMail mail = new(email, "Reset Password Link : " + url, "admin");
			await mail.SendMailAsync();
			TempData["errors"] = JsonConvert.SerializeObject(new List<ConfirmError>() { new ConfirmError() { AlertType = "success", Description = "Reset password Link has been gone to your mail" } });
			return RedirectToAction("Index","Login");
		}
		public IActionResult ResetPassword(ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
				return View("ResetPassword",model);
			return View("ResetPassword", model);
		}

		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model,string new_pass,string renew_pass)
		{
			if (!ModelState.IsValid)
				return View("ResetPassword", model);
			if (string.IsNullOrEmpty(new_pass) || string.IsNullOrEmpty(renew_pass))
			{
				ModelState.AddModelError("","Password is not null");
				return View("ResetPassword", model);
			}
			if (new_pass != renew_pass)
			{
				ModelState.AddModelError("","Parolalar eşleşmiyor...");
				return View("ResetPassword", model);
			}
			var user = await _userManager.FindByIdAsync(model.Id.ToString());
			var confirmpass = await _userManager.ResetPasswordAsync(user, model.ConfirmCode, new_pass);
			if (confirmpass.Succeeded)
				return RedirectToAction("Index", "Login");
			foreach (var item in confirmpass.Errors)
				ModelState.AddModelError("",item.Description);
			return View("ResetPassword", model);
		}
		public async Task<IActionResult> Logout()
		{
			 await _signInManager.SignOutAsync();
			return RedirectToAction("Index","Login");
		}
		public async Task<IActionResult> ChangePassword(string? slug)
		{
			var errors = new List<Error>();
			if (slug == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Slug is required..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			var user = await _staffService.FindBySlugAsync(slug);
			if (user == null)
			{
                errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded" });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Admin");
            }
			return View(new ChangePasswordModel()
			{
				UserId = user.Id
			});
		}
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			var errors = new List<Error>();
			if (!ModelState.IsValid)
				return View(model);
			var user = await _userManager.FindByIdAsync(model.UserId.ToString());
			if (user == null)
			{
				ModelState.AddModelError("","User has been not founded...");
				return View(model);
			}
			var passwordValidate = await _userManager.CheckPasswordAsync(user, model.OldPassword);
			if (!passwordValidate)
			{
				ModelState.AddModelError("","Old Password has not corrected...");
				return View(model);
			}
			var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (changePassword.Succeeded)
			{
				errors.Add(new Error() { AlertType = "warning",Description = "Password Changed..."});
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			foreach (var item in changePassword.Errors)
				ModelState.AddModelError("",item.Description);
			return View(model);
		}
	}
}
