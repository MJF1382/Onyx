using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Onyx.Classes;
using Onyx.Models.Identity.Entities;
using Onyx.Models.ViewModels;
using Onyx.Services;
using System.Security.Claims;

namespace Onyx.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMailSender _mailSender;

        public AccountController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMailSender mailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailSender = mailSender;
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = "/")
        {
            RegisterViewModel viewModel = new RegisterViewModel()
            {
                ReturnUrl = returnUrl
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
        public IActionResult SignIn(string returnUrl = "/")
        {
            LoginViewModel viewModel = new LoginViewModel()
            {
                ReturnUrl = returnUrl
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
                    var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, true);

                    if (result.Succeeded)
                    {
                        return LocalRedirect(viewModel.ReturnUrl);
                    }
                    else if (result.IsLockedOut)
                    {
                        return RedirectToAction("LockedOut");
                    }
                    else
                    {
                        ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "نام کاربری یا رمز عبور اشتباه است");
                }
            }
            else
            {
                ModelState.AddModelError("", "اطلاعات را وارد کنید");
            }

            return View(viewModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            string callbackUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl }, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, callbackUrl);

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/", string? remoteError = null)
        {
            if (remoteError == null)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();

                if (info != null)
                {
                    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, info.AuthenticationProperties.IsPersistent);

                    if (result.Succeeded)
                    {
                        var updateResult = await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                        if (updateResult.Succeeded)
                        {
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            foreach (IdentityError error in updateResult.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        string email = info.Principal.FindFirstValue(ClaimTypes.Email);

                        return RedirectToAction("ExternalLoginConfirmation", new { email = email, returnUrl = returnUrl });
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", remoteError);
            }

            return View();
        }

        [HttpGet]
        public IActionResult ExternalLoginConfirmation(string email, string returnUrl = "/")
        {
            ExternalLoginConfirmationViewModel model = new ExternalLoginConfirmationViewModel()
            {
                Email = email,
                UserName = email.Split("@")[0],
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    FullName = viewModel.FullName,
                    Email = viewModel.Email,
                    UserName = viewModel.UserName
                };

                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    var info = await _signInManager.GetExternalLoginInfoAsync();

                    if (info != null)
                    {
                        result = await _userManager.AddLoginAsync(user, info);

                        if (result.Succeeded)
                        {
                            var externalSignInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, info.AuthenticationProperties.IsPersistent);

                            if (externalSignInResult.Succeeded)
                            {
                                result = await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                                if (result.Succeeded)
                                {
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
                        }
                        else
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
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

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            ViewBag.IsSubmitted = false;

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
                string? callbackUrl = Url.Action("ResetPassword", "Account", new { userName = viewModel.UserName, token = token }, Request.Scheme);

                Email email = new Email()
                {
                    To = user.Email,
                    Subject = "بازیابی رمز عبور",
                    Body = "<b>برای بازیابی رمز عبور روز لینک زیر کلیک کنید:</b>" + "<br>" + $"<a href='{callbackUrl}'>بازیابی</a>"
                };

                await _mailSender.SendAsync(email);
            }
            else
            {
                ModelState.AddModelError("", "کاربر مورد نظر یافت نشد.");
            }

            ViewBag.IsSubmitted = true;

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ResetPassword(string userName, string token)
        {
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel()
            {
                UserName = userName,
                Token = token
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(viewModel.UserName);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);

                    if (result.Succeeded == false)
                    {
                        return RedirectToAction("SignIn");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return View();
        }

        public IActionResult LockedOut()
        {
            return View();
        }
    }
}
