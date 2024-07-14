using AKUWebUI.MessageService;
using AKUWebUI.Models.Permission;
using AKUWebUI.Models.Staff;
using AKUWebUI.Models.StaffPermission;
using AKUWebUI.Views.Shared;
using BusinessLayer.Abstract.EFCore;
using EntityLayer;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AKUWebUI.Controllers
{
	[Authorize(Roles =$"{nameof(Rol.SuperAdmin)},{nameof(Rol.Admin)}")]
	public class StaffsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreStaffService _staffService;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IEFCorePermissionService _permissonService;
		private readonly IEFCoreStaffPermissionService _staffPermissionService;
		private readonly WebContext _context;
		private List<Error> errors = new();
		public StaffsController(UserManager<AppUser> userManager, IEFCoreStaffService staffService, SignInManager<AppUser> signInManager, IEFCorePermissionService permissionService, IEFCoreStaffPermissionService staffPermissionService, WebContext context)
		{
			_userManager = userManager;
			_staffService = staffService;
			_signInManager = signInManager;
			_permissonService = permissionService;
			_staffPermissionService = staffPermissionService;
			_context = context;
		}
		private IActionResult AddError(Error error)
		{
			errors.Add(error);
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return Redirect("/Staffs");
		}

		public async Task<IActionResult> Index()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			ViewBag.Role = user.Role < Rol.Teacher;
			ViewBag.UserId = user.Id;
			if (user.Role == Rol.Teacher)
				return View(await _staffService.GetAllFilteredAsync(s => s.Role == Rol.Teacher && s.BranchId == user.BranchId));
			else if (user.Role == Rol.Admin)
				return View(await _staffService.GetAllFilteredAsync(s => (s.Role == Rol.Admin || s.Role == Rol.Teacher) && s.BranchId == user.BranchId));
			return View(await _staffService.GetAllAsync());
		}
		public async Task<IActionResult> DeleteStaff(string slug)
		{
			var staff = await _staffService.FindBySlugAsync(slug);
			if (staff == null)
				return AddError(new Error() { AlertType ="danger", Description = "Personel Bulunamadı..."});
			await _userManager.DeleteAsync(staff);
			return AddError(new Error() { AlertType = "success", Description = "Personel Silindi..." });
				
		}
		public async Task<IActionResult> ShowHistoryPayments(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Slug boş geçilemez..." });
			var staff = await _staffService.FindBySlugAsync(slug);
			if (staff == null)
				return AddError(new Error() { AlertType = "danger", Description = "Personel Bulunamadı..." });
			return View(await _context.StaffPermissions.Include(s => s.AppUser).Where(s => s.StaffId == staff.Id).ToListAsync());
		}
		public async Task<IActionResult> ChangeImagePath(string slug)
		{
			if (string.IsNullOrEmpty(slug))
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..."});
			var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Slug == slug);
			if (user == null)
				return AddError(new Error() { AlertType = "danger", Description = "Personel Bulunamadı..."});
			return View(new ChangeImageModel() {  UserId = user.Id});
		}
		[HttpPost]
		public async Task<IActionResult> ChangeImagePath(ChangeImageModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			_context.ChangeTracker.Clear();
			var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == model.UserId);
			if (user == null)
				return AddError(new Error() { AlertType = "danger", Description = "Personel Bulunamadı..."});
			if (model.ImagePath != null)
			{
				var filePath = Path.Combine("wwwroot/Images/", user.ImagePath == null ? "" : user.ImagePath);

				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);
				var uploadsFolder = Path.Combine("wwwroot/", "Images");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var imagePath = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
				var _filePath = Path.Combine(uploadsFolder, imagePath);

				using (var stream = new System.IO.FileStream(_filePath, FileMode.Create))
				{
					await model.ImagePath.CopyToAsync(stream);
				}
				user.ImagePath = imagePath;
			}
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
			return AddError(new Error() { AlertType = "danger", Description = "Personel Güncellendi"});
			;
		}
		public async Task<IActionResult> RemoveStaffPermission(int? id)
		{
			if (id == null)
				return AddError(new Error() { AlertType ="danger", Description = "Parametre Hatası"});
			var staffPermission = await _staffPermissionService.GetByIdAsync((int)id);
			if (staffPermission == null)
				return AddError(new Error() { Description ="İzin Bulunamadı...", AlertType = "danger"});
			_staffPermissionService.Delete(staffPermission);
			return AddError(new Error() { AlertType = "success", Description = "İzin Silindi..."});
			
		}

        public async Task<IActionResult> Details(string? name)
		{
			var errors = new List<Error>();
			if (name == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "SlugName is required..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			var user = await _staffService.FindBySlugAsync(name);
			if (user == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded" });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			ViewBag.Slug = user.Slug;
			var staffRole = (await _userManager.FindByNameAsync(User.Identity.Name)).Role;
			var userRole = (await _staffService.FindBySlugAsync(name)).Role;
			ViewBag.Role = false;
			if (userRole > staffRole)
				ViewBag.Role = true;
			TimeSpan year = DateTime.Now - user.Date;
			byte age = (byte)(year.TotalDays / 365);
			return View(new ResultStaffModel()
			{
				UserId = user.Id,
				Email = user.Email,
				Gender = user.Gender,
				Name = user.Name,
				Surname = user.Surname,
				Date = user.Date,
				Phone = user.PhoneNumber,
				TC = user.TC,
				Age = age,
				BranchName = user.Branch.BranchName,
				Role = user.Role,
				ImagePath = user.ImagePath
			});
		}
		public async Task<IActionResult> EditStaff(string? slug)
		{
			var errors = new List<Error>();
			var user = await _staffService.FindBySlugAsync(slug);
			if (user == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded!" });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			return View(new UpdateStaffModel()
			{
				Date = user.Date,
				StaffId = user.Id,
				Name = user.Name,
				Surname = user.Surname,
				TC = user.TC,
				Phone = user.PhoneNumber,
				Email = user.Email
			});
		}
		[HttpPost]
		public async Task<IActionResult> EditStaff(UpdateStaffModel model)
		{
			var errors = new List<Error>();
			if (!ModelState.IsValid)
				return View(model);
			var user = await _userManager.FindByIdAsync(model.StaffId.ToString());
			if (user == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			if (model.Email != user.Email && model.Email != "")
			{
				var emailValidate = await _context.Users.Where(u => u.Id != user.Id && u.Email == model.Email).AsNoTracking().ToListAsync();
				if (emailValidate.Count > 0)
				{
					ModelState.AddModelError("", "Email Kullanılıyor...");
					return View(model);
				}

				user.Email = model.Email;
				user.EmailConfirmed = false;
				var mailLink = Url.ActionLink("EmailConfirmed", "Register", new
				{
					Id = user.Id,
					ConfirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user)
				});
				SendMail mail = new(model.Email, mailLink, "Mail Onay Linki");
				await mail.SendMailAsync();
			}
			if (user.PhoneNumber != model.Phone)
			{
				var phoneValidate = await _context.Users.Where(u => u.Id != user.Id && u.PhoneNumber == model.Phone).AsNoTracking().ToListAsync();
				if (phoneValidate.Count > 0)
				{
					ModelState.AddModelError("", "Telefon Numarası zaten kullanılıyor...");
					return View(model);
				}
				user.PhoneNumber = model.Phone;
			}
			if (model.Name.ToLower() != user.Name.ToLower() || model.Surname.ToLower() != user.Surname.ToLower())
			{
				var validate = await _context.Users.Where(u => (u.Name + u.Surname).Replace(" ", "-").ToLower() == (model.Name + model.Surname).Replace(" ", "-").ToLower() && u.Id != user.Id).AsNoTracking().ToListAsync();
				if (validate.Count > 0)
				{
					ModelState.AddModelError("", "Bu İsim Soyisim zaten kullanılıyor...");
					return View(model);
				}
				user.Name = model.Name;
				user.Surname = model.Surname;
			}
			if (model.TC != user.TC)
			{
				var validate = await _context.Users.Where(u => u.Id != user.Id && model.TC == u.TC).AsNoTracking().ToListAsync();
				if (validate.Count > 0)
				{
					ModelState.AddModelError("", "TC zaten kullanılıyor...");
					return View(model);
				}
				user.TC = model.TC;
			}

			user.Date = model.Date;
			user.Slug = model.Name.Replace(" ", "-") + "-" + model.Surname.Replace(" ", "-");

			if (model.ImagePath != null)
			{
				var filePath = Path.Combine("wwwroot/Images/", user.ImagePath);

				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);

				var uploadsFolder = Path.Combine("wwwroot/", "Images");
				if (!Directory.Exists(uploadsFolder))
					Directory.CreateDirectory(uploadsFolder);

				var imagePath = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
				var _filePath = Path.Combine(uploadsFolder, imagePath);

				using (var stream = new System.IO.FileStream(_filePath, FileMode.Create))
				{
					await model.ImagePath.CopyToAsync(stream);
				}
				user.ImagePath = imagePath;
			}

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				foreach (var item in result.Errors)
				{
					ModelState.AddModelError("", item.Description);
				}
				return View(model);
			}


			errors.Add(new Error() { AlertType = "warning", Description = "Personel Güncellendi" });
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Index", "Staffs");
		}

		public async Task<IActionResult> EditEmail(string? slug)
		{
			var errors = new List<Error>();
			if (slug == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Slug must be not null..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Staffs");
			}
			var user = await _staffService.FindBySlugAsync(slug);
			if (user == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return RedirectToAction("Index", "Staffs");
			}
			return View(new UpdateEmailStaffModel() { StaffId = user.Id });
		}
		[HttpPost]
		public async Task<IActionResult> EditEmail(UpdateEmailStaffModel model)
		{
			var errors = new List<Error>();
			var staff = await _userManager.FindByNameAsync(User.Identity.Name);
			if (!ModelState.IsValid)
				return View(model);
			var user = await _userManager.FindByIdAsync(model.StaffId.ToString());
			if (user == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "User has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Staffs");
			}
			if ((await _staffService.GetAllAsync()).Any(u => u.Email == model.Email))
			{
				ModelState.AddModelError("", "Email has been already used...");
				return View(model);
			}
			user.Email = model.Email;
			user.EmailConfirmed = false;
			await _userManager.UpdateAsync(user);
			var mailLink = Url.ActionLink("EmailConfirmed", "Register", new
			{
				Id = user.Id,
				ConfirmCode = await _userManager.GenerateEmailConfirmationTokenAsync(user)
			});
			SendMail mail = new(model.Email, $"Mail Onay Link : {mailLink}", "Mail Onay Link");
			await mail.SendMailAsync();
			errors.Add(new Error() { AlertType = "warning", Description = "EmailConfirm link has just been sent your mailadress..." });
			errors.Add(new Error() { AlertType = "warning", Description = "Log In again" });
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			if (staff.UserName == user.UserName)
			{
				await _signInManager.SignOutAsync();
				return Redirect("/");
			}
			return RedirectToAction("Index", "Staffs");
		}
		public async Task<IActionResult> AddPermission(string slug)
		{
			if (slug == null)
				return AddError(new Error() { AlertType = "danger", Description = "Parametre Hatası..." });
			var staff = await _staffService.FindBySlugAsync(slug);
			if (staff == null)
				return AddError(new Error() { AlertType = "danger", Description = "Personel Bulunamadı..." });
			var permissions = await _permissonService.GetAllIncludeAsync();
			if (permissions.Count == 0)
				return AddError(new Error() { AlertType = "danger", Description = "İzin Bulunamadı..." });
			foreach (var item in permissions)
			{
				var nextpermission = await _permissonService.FindNextByIdAsync(item.PermissionId, item.YearCount);
				var time = (int)((DateTime.Now - staff.EntryJobDate).TotalDays / 365);
				if (nextpermission == null)
				{
					if (time >= item.YearCount)
					{
						var count = await _context.StaffPermissions.Where(s => s.StaffId == staff.Id && s.Date.Year == DateTime.Now.Year).SumAsync(s => s.RestPermissionCount);
						if (item.DayCount < count)
							return AddError(new Error() { AlertType = "danger", Description = "İzin Hakkınız Dolmuştur..." });
						return View(new CreateStaffPermissionModel()
						{
							UserId = staff.Id,
							DayCount = item.DayCount,
							PermissionId = item.PermissionId,
							OwnPermissionCount = item.DayCount - count
						});
					}

				}
				else
				{
					if (time >= item.YearCount && time <= nextpermission.YearCount)
					{
						var count = await _context.StaffPermissions.Where(s => s.StaffId == staff.Id && s.Date.Year == DateTime.Now.Year).SumAsync(s => s.RestPermissionCount);
						if (item.DayCount < count)
							return AddError(new Error() { AlertType = "danger", Description = "İzin Hakkınız Dolmuştur..." });
						return View(new CreateStaffPermissionModel() { UserId = staff.Id, DayCount = item.DayCount, PermissionId = item.PermissionId, OwnPermissionCount = item.DayCount - count });
					}
				}
			}
			var permission = (await _permissonService.GetAllAsync()).Min(p => p.YearCount);
			return AddError(new Error() { AlertType = "danger", Description = $"{permission} yıl  süreyi doldurmadan izin kullamazsınız..." });
		}
		[HttpPost]
		public async Task<IActionResult> AddPermission(CreateStaffPermissionModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var staff = await _staffService.GetByIdAsync(model.UserId);
			var permission = await _permissonService.GetByIdAsync(model.PermissionId);

			var count = (await _staffPermissionService.GetAllFilteredAsync(s => s.StaffId == staff.Id && model.Date.Year == DateTime.Now.Year)).Sum(s => s.RestPermissionCount) + model.DayCount;
			if (model.Date.Date <= DateTime.Now.Date)
			{
				ModelState.AddModelError("","Tarih Bilgisi Hatalıdır...");
				return View(model);
			}
			if (permission.DayCount < count)
			{
				ModelState.AddModelError("", "İzin Hak sınırı aşıldı...");
				return View(model);
			}
			await _staffPermissionService.AddAsync(new StaffPermission()
			{
				StaffId = staff.Id,
				RestPermissionCount = model.DayCount,
				Date = model.Date
			});
			return AddError(new Error() { AlertType = "danger", Description = "İzin Eklendi..." });
		}
	}
}
