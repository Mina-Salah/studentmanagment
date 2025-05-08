using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Core.Entities;


namespace studentmanagment.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		
	}
}
