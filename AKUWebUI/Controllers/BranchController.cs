using AKUWebUI.Models.Admin;
using BusinessLayer.Abstract.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EntityLayer.Entities;
using Newtonsoft.Json;
using AKUWebUI.Views.Shared;
using Microsoft.AspNetCore.Identity;
using EntityLayer;

namespace AKUWebUI.Controllers
{
    [Authorize]
    public class BranchController : Controller
    {
        private readonly IEFCoreBranchService _branchService;
        private readonly UserManager<AppUser> _userManager;
        List<Error> errors = new();

		public BranchController(IEFCoreBranchService branchService,UserManager<AppUser> userManager)
		{
			_branchService = branchService;
            _userManager = userManager; 
		}

		public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userRole = (await _userManager.GetRolesAsync(user))[0];
            ViewBag.Role = userRole == Rol.SuperAdmin.ToString() ? true : false;
            var branches = await _branchService.GetAllAsync();
            return View(branches);
        }
        [NonAction]
        public IActionResult AddError(Error error)
        {
            errors.Add(error);
            TempData["Errors"] = JsonConvert.SerializeObject(errors);
            return Redirect("/Branch");
		}
        [Authorize(Roles =$"{nameof(Rol.SuperAdmin)}")]
        public IActionResult CreateBranch()
        {
            return View();
        }
        [Authorize(Roles = $"{nameof(Rol.SuperAdmin)}")]
        [HttpPost]
        public async Task<IActionResult> CreateBranch(CreateBranch model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var validate = await _branchService.GetAllFilteredAsync(b => b.BranchName.ToLower() == model.BranchName);
            if (validate.Count > 0)
            {
                ModelState.AddModelError("","Şube zaten kullanılıyor...");
                return View(model);
            }
               
            await _branchService.AddAsync(new Branch() { BranchName = model.BranchName });
            var errors = new List<Error>() { new Error() { AlertType="success" ,Description = "Branch added"} };
            TempData["Errors"] = JsonConvert.SerializeObject(errors);
            return RedirectToAction("Index","Branch");
        }
        [Authorize(Roles = $"{nameof(Rol.SuperAdmin)}")]
        public async Task<IActionResult> DeleteBranch(int? id)
        {
            var errors = new List<Error>();
            if (id == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "BranchId is required" });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Branch");
            }
            var branch = await _branchService.GetByIdAsync((int)id);
            if (branch == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "Branch has been not found..." });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Branch");
            }
             _branchService.Delete(branch);
            errors.Add(new Error() { AlertType = "success", Description = "Branch has been deleted..." });
            TempData["Errors"] = JsonConvert.SerializeObject(errors);
            return Redirect("/Branch");
        }
        [Authorize(Roles = $"{nameof(Rol.SuperAdmin)}")]
        public async Task<IActionResult> UpdateBranch(int? id)
        {
            var errors = new List<Error>();
            if (id == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "Branch is required..." });
                TempData["Errors"] = errors;
                return Redirect("/Branch");
            }
            var branch = await _branchService.GetByIdAsync((int)id);
            if (branch == null)
            {
                errors.Add(new Error() { AlertType = "danger", Description = "Branch has been not found..." });
                TempData["Errors"] = JsonConvert.SerializeObject(errors);
                return Redirect("/Branch");
            }
            return View(new UpdateBranch() { BranchId = branch.BranchId,BranchName = branch.BranchName});
        }
        [Authorize(Roles = $"{nameof(Rol.SuperAdmin)}")]
        [HttpPost]
        public async Task<IActionResult> UpdateBranch(UpdateBranch model)
        {
            var errors = new List<Error>();
            if (!ModelState.IsValid)
                return View(model);
            var validate = await _branchService.GetAllFilteredAsync(b => b.BranchId != model.BranchId && b.BranchName.ToLower() == model.BranchName.ToLower());
            if (validate.Count > 0)
            {
                ModelState.AddModelError("","Şube adı kullanılıyor....");
                return View(model);
            }
            var branch = await _branchService.GetByIdAsync(model.BranchId);
            branch.BranchName = model.BranchName;
            _branchService.Update(branch);
            errors.Add(new Error() { AlertType = "success", Description = "Branch updated" });
            TempData["Errors"] = JsonConvert.SerializeObject(errors);
            return Redirect("/Branch");
        }
    }
}
