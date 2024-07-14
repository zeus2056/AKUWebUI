using AKUWebUI.Models.Parent;
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
	[Authorize]
	public class ParentsController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEFCoreParentService _parentService;
		private readonly IEFCoreStudentService _studentService;
		private List<Error> errors = new();
		private readonly WebContext _context;
		public ParentsController(UserManager<AppUser> userManager, IEFCoreParentService parentService, IEFCoreStudentService studentService, WebContext context)
		{
			_userManager = userManager;
			_parentService = parentService;
			_studentService = studentService;
			_context = context;
		}
		[Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
		public async Task<IActionResult> CreateParent(string? slug)
		{
			if (slug == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Slug is required..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			var student = await _studentService.GetUserBySlugAsync(slug);
			if (student == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Student has been not foundend..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			return View(new CreateParentModel() { StudentId = student.StudentId });
		}
		[HttpPost]
		[Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
		public async Task<IActionResult> CreateParent(CreateParentModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var student = await _studentService.GetByIdAsync(model.StudentId);
			if (student == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Student has been not foundend..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			var parent = new Parent()
			{
				Job = model.Job,
				Mail = model.Mail,
				Name = model.Name,
				StudentId = model.StudentId,
				ParentType = model.ParentType,
				PhoneNumber = model.PhoneNumber,
				Surname = model.Surname,
				TC = model.TC,
				Slug = model.Name.Replace(" ", "-") + "-" + model.Surname.Replace(" ", "-")
			};
			var validate = (await _parentService.GetAllAsync()).Count > 0 ? (await _parentService.GetAllAsync()).Any(p => p.Slug.ToUpper() != parent.Slug.ToUpper()) : true;
			if (!validate)
			{
				ModelState.AddModelError("", "FullName has already been used...");
				return View(model);
			}
			await _parentService.AddAsync(parent);
			return RedirectToAction("Details", "Students", new
			{
				name = student.Slug
			});

		}
		[Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
		public async Task<IActionResult> DeleteParent(string? slug)
		{
			var parent = await _parentService.FindBySlugAsync(slug);
			if (parent == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Parent has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			_parentService.Delete(parent);
			errors.Add(new Error() { AlertType = "warning", Description = "Parent Deleted...." });
			TempData["Errors"] = JsonConvert.SerializeObject(errors);
			return RedirectToAction("Detail", "Students", new
			{
				name = parent.Student.Slug
			});
		}
		[Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
		public async Task<IActionResult> EditParent(string? slug)
		{
			if (slug == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Slug is required..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Students");
			}
			var parent = await _parentService.FindBySlugAsync(slug);
			if (parent == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Parent has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Students");
			}
			return View(new EditParentModel()
			{
				Job = parent.Job,
				Mail = parent.Mail,
				Name = parent.Name,
				ParentId = parent.ParentId,
				ParentType = parent.ParentType,
				PhoneNumber = parent.PhoneNumber,
				Surname = parent.Surname,
				TC = parent.TC
			});
		}
		[Authorize(Roles = $"{nameof(Rol.Admin)},{nameof(Rol.SuperAdmin)}")]
		[HttpPost]
		public async Task<IActionResult> EditParent(EditParentModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
			var parent = await _parentService.FindByIdWithStudentAsync(model.ParentId);
			if (parent == null)
			{
				errors.Add(new Error() { AlertType = "warning", Description = "Parent has been not founded..." });
				TempData["Errors"] = JsonConvert.SerializeObject(errors);
				return Redirect("/Admin");
			}
			var tcValidate = await _context.Parents.AsNoTracking().FirstOrDefaultAsync(p => p.ParentId != model.ParentId && p.TC == model.TC);
			if (tcValidate != null)
			{
				ModelState.AddModelError("", "TC kullanılıyor...");
				return View(model);
			}
			var phoneValidate = await _context.Parents.AsNoTracking().FirstOrDefaultAsync(p => p.ParentId != model.ParentId && p.PhoneNumber == model.PhoneNumber);
			if (phoneValidate != null)
			{
				ModelState.AddModelError("", "Telefon Numarası kullanılıyor...");
				return View(model);
			}
			parent.TC = model.TC;
			parent.Name = model.Name;
			parent.Surname = model.Surname;
			parent.Mail = model.Mail;
			parent.PhoneNumber = parent.PhoneNumber;
			parent.Job = model.Job;
			parent.ParentType = model.ParentType;
			_parentService.Update(parent);
			return RedirectToAction("Details", "Students", new
			{
				name = parent.Student.Slug
			});
		}
	}
}
