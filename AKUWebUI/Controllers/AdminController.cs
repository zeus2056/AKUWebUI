using AKUWebUI.Models.Admin;
using BusinessLayer.Abstract.EFCore;
using DataAccessLayer.Abstract.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKUWebUI.Controllers
{
	[Authorize]
	public class AdminController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
