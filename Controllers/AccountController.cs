using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Onyx.Models.Identity.Entities;
using Onyx.Models.ViewModels;

namespace Onyx.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            LoginViewModel viewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(viewModel.UserName);
                var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);

                if (result.Succeeded)
                {

                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            RegisterViewModel viewModel = new RegisterViewModel()
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    Email = viewModel.Email,
                    UserName = viewModel.PhoneNumber,
                    FullName = viewModel.FullName,
                    PhoneNumber = viewModel.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, viewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    return LocalRedirect(viewModel.ReturnUrl);
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(viewModel);
        }
    }
}
