
using AKUWebUI.MessageService;
using AKUWebUI.Models.Register;
using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles =$"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
	public class RegisterController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreBranchService _branchService;

		public RegisterController(UserManager<AppUser> userManager,IEFCoreBranchService branchService,RoleManager<AppRole> roleManager)
		{
			_userManager = userManager;
			_branchService = branchService;
		}

		public async Task<IActionResult> Index()
		{
			
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = (await _userManager.GetRolesAsync(user)).Any(r => r != Rol.Admin.ToString());
			if (ViewBag.Role)
				ViewBag.Branches = await _branchService.GetAllAsync();
				
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Index(RegisterModel model)
		{
            var _user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.Role = (await _userManager.GetRolesAsync(_user)).Any(r => r != Rol.Admin.ToString());
			if (ViewBag.Role)
                ViewBag.Branches = await _branchService.GetAllAsync();
                
            if (!ModelState.IsValid)
				return View(model);
			var usernameValidate = await _userManager.FindByNameAsync(model.UserName);
			var emailValidate = await _userManager.FindByEmailAsync(model.Email);
			if (usernameValidate != null || emailValidate != null)
			{
				ModelState.AddModelError("","Email or Username has already been used");
				return View(model);
			}
			var appUser = new AppUser()
			{
				Email = model.Email,
				Name = model.Name,
				Surname = model.Surname,
				PhoneNumber = model.Phone,
				UserName = model.UserName,
				Gender = model.Gender,
				TC = model.TC,
				Date = model.Date,
				Slug = model.Name.Replace(" ", "-") + "-" + model.Surname.Replace(" ", "-"),
				Role = model.Role,
				BranchId = ViewBag.Role ? model.BranchId : _user.BranchId,
				EntryJobDate = DateTime.Now
			};
			if (model.ImagePath != null)
			{
				var imagePath = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
				var uploadsFolder = Path.Combine("wwwroot/", "Images");
				var filePath = Path.Combine(uploadsFolder, imagePath);
				using (var stream = new System.IO.FileStream(filePath, FileMode.Create))
				{
					await model.ImagePath.CopyToAsync(stream);
				}
				appUser.ImagePath = imagePath;
			}
			var validate = await _userManager.CreateAsync(appUser, model.Password);

			if (validate.Succeeded)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				var mailLink = Url.ActionLink("EmailConfirmed", "Register", new
				{
					Id = user.Id,
					ConfirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user)
				});
				TempData["ConfirmErrors"] = JsonConvert.SerializeObject(new List<ConfirmError>()
				{
					new ConfirmError(){AlertType = "success",Description = "Confirm Code has been gone your mail"}
				});
				SendMail mail = new(model.Email,mailLink,"Mail Onaylama Linki");
				await mail.SendMailAsync();
				await _userManager.AddToRoleAsync(user, model.Role.ToString());
                return Redirect("/Staffs");

            }
			foreach (var item in validate.Errors)
				ModelState.AddModelError("",item.Description);
			return View(model);
		}
		[AllowAnonymous]
		public async Task<IActionResult> EmailConfirmed(EmailConfirmModel model)
		{
			List<ConfirmError> errors = new List<ConfirmError>();
			if (!ModelState.IsValid)
			{
				errors.Add(new ConfirmError() { AlertType = "danger",Description = "Id or ConfirmCode is not null"});
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Login");
            }
				
			var user = await _userManager.FindByIdAsync(model.Id.ToString());
			var validate = await _userManager.ConfirmEmailAsync(user, model.ConfirmCode);
			if (validate.Succeeded)
			{
				errors.Add(new ConfirmError() { AlertType="success",Description= "Email Confirmed"});
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Login");
            }
			foreach (var item in validate.Errors)
				errors.Add(new ConfirmError() { AlertType = "danger",Description = item.Description});
            return Redirect("/Login");
        }
	}
}
