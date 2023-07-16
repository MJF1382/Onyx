using Microsoft.AspNetCore.Mvc;

namespace Onyx.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
