using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Onyx.Classes;
using Onyx.Models.Identity.Entities;
using Onyx.Models.ViewModels;
using Onyx.Services;

namespace Onyx.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailSender _mailSender;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IMailSender mailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mailSender = mailSender;
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

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        return LocalRedirect(viewModel.ReturnUrl);
                    }
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(viewModel.UserName);

            if (user != null)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                string? callbackUrl = Request.Scheme + "://" + Request.Host.Value + Url.Action("ResetPassword", "Account", new { userName = viewModel.UserName, token = token });

                Email email = new Email()
                {
                    To = user.Email,
                    Subject = "بازیابی رمز عبور",
                    Body = "برای بازیابی رمز عبور روز لینک زیر کلیک کنید:" + "<br>" + "<a href='" + callbackUrl + "'>بازیابی</a>"
                };

                await _mailSender.SendAsync(email);
            }
            else
            {
                ModelState.AddModelError("", "کاربر مورد نظر یافت نشد.");
            }

            return View(viewModel);
        }
    }
}
