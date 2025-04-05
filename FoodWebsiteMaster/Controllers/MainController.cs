using Microsoft.AspNetCore.Mvc;

namespace FoodWebsiteMaster.Controllers
{
	public class MainController : Controller
	{
		public IActionResult Home()
		{
			return View();
		}
		public IActionResult Home2()
		{
			return View();
		}
	}
}
