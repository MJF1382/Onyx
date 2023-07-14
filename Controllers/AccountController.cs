using Microsoft.AspNetCore.Mvc;
using Onyx.Models.ViewModels;

namespace Onyx.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn(string returnUrl)
        {
            RegisterViewModel viewModel = new RegisterViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
