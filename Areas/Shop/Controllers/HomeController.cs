using Microsoft.AspNetCore.Mvc;

namespace Assignment.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
