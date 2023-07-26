using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Onyx.Classes;
using Onyx.Models.Identity.Entities;
using Onyx.Models.ViewModels;

namespace Onyx.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AdminController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index(string? popUpMessage = null)
        {
            ViewBag.PopUpMessage = popUpMessage;

            return View();
        }

        #region User

        public IActionResult UserIndex()
        {
            var model = _userManager.Users.Select(user => new UserViewModel()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Password = user.PasswordHash ?? "",
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.RolesList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserViewModel viewModel)
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
                    result = await _userManager.AddToRolesAsync(user, viewModel.RolesName);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", new { popUpMessage = "کاربر با موفقیت افزوده شد." });
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
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

        [HttpGet]
        public async Task<IActionResult> EditUser(string userName)
        {
            if (userName.HasValue())
            {
                var user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    EditUserViewModel model = new EditUserViewModel();
                    model.Id = user.Id;
                    model.FullName = user.FullName;
                    model.RolesName = (await _userManager.GetRolesAsync(user)).ToList();
                    model.IsExternalLogin = !user.PasswordHash.HasValue();

                    if (user.PasswordHash.HasValue())
                    {
                        model.Email = user.Email;
                        model.PhoneNumber = user.PhoneNumber;
                    }

                    ViewBag.RolesList = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

                    return View(model);
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByIdAsync(viewModel.Id);

                user.FullName = viewModel.FullName;
                var result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRolesAsync(user, viewModel.RolesName);

                    if (result.Succeeded)
                    {
                        result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            if (viewModel.IsExternalLogin == false)
                            {
                                user.PhoneNumber = viewModel.PhoneNumber;
                                user.Email = viewModel.Email;
                                user.UserName = viewModel.PhoneNumber;
                                result = await _userManager.RemovePasswordAsync(user);

                                if (result.Succeeded)
                                {
                                    result = await _userManager.AddPasswordAsync(user, viewModel.Password);

                                    if (result.Succeeded)
                                    {
                                        return RedirectToAction("Index", new { popUpMessage = "کاربر با موفقیت ویرایش شد." });
                                    }
                                    else
                                    {
                                        foreach (IdentityError error in result.Errors)
                                        {
                                            ModelState.AddModelError("", error.Description);
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

                            return RedirectToAction("Index", new { popUpMessage = "کاربر با موفقیت ویرایش شد." });
                        }
                        else
                        {
                            foreach (IdentityError error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { popUpMessage = "کاربر با موفقیت حذف شد." });
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Index", new { popUpMessage = "خطا در اتصال به سرور، لطف ابعدا دوباره امتحان کنید." });
        }

        #endregion

        #region Role

        public async Task<IActionResult> RoleIndex()
        {
            var model = await _roleManager.Roles.Select(role => new RoleViewModel()
            {
                Id = role.Id,
                Name = role.Name
            }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(viewModel.Name));

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { popUpMessage = "نقش با موفقیت افزوده شد." });
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
        public async Task<IActionResult> EditRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                RoleViewModel model = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name
                };

                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(RoleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = await _roleManager.FindByIdAsync(viewModel.Id);
                role.Name = viewModel.Name;

                if (role != null)
                {
                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", new { popUpMessage = "نقش با موفقیت ویرایش شد." });
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", new { popUpMessage = "نقش با موفقیت حذف شد." });
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("Index", new { popUpMessage = "خطا در اتصال به سرور، لطف ابعدا دوباره امتحان کنید." });
        }

        #endregion
    }
}
