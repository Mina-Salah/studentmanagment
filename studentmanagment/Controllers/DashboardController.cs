using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;


namespace studentmanagment.Controllers
{
	[Authorize]
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		
	}
}
