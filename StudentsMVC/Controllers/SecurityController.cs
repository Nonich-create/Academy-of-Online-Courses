using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace Students.MVC.Controllers
{
    public class SecurityController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ILogger<SecurityController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Отображения редактирования пользователя
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Security(string id)
        {
            _logger.LogInformation("Requested the Security Page Security");
            try
            {
                var user = await _userService.GetAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
 
        }
        #endregion
        #region Редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Security(ApplicationUser model)
        {
            _logger.LogInformation("Requested the Security Page Security post");
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.PhoneNumber = model.PhoneNumber;
                    try
                    {
                        await _userManager.UpdateAsync(user);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (await _userService.ExistsAsync(model.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
            
        }
        #endregion
        #region Отображения смены пароля пользователя
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> ChangePassword(string id)
        {
            _logger.LogInformation("Requested the ChangePassword Page Security");
            try
            {
                var user = await _userService.GetAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var model = new ChangePasswordViewModel()
                {
                    UserIdentityId = user.Id
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
 
        }
        #endregion
        #region Смена пароля
        [HttpPost]
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            _logger.LogInformation("Requested the ChangePassword Page Security post");
            try
            {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.UserIdentityId);
                    if (user != null)
                    {
                        var _passwordValidator =
                            HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
                        var _passwordHasher =
                            HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

                        IdentityResult result =
                            await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                        if (result.Succeeded)
                        {
                            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                            await _userManager.UpdateAsync(user);
                            return Redirect(Request.Headers["Referer"].ToString());
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Пользователь не найден");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
          
        }
        #endregion
        #region Отображения авторизация 
        [HttpGet]
        public  IActionResult Authorization(string returnUrl = null)
        {
            _logger.LogInformation("Requested the Authorization Page Security");
            try
            {
                return View(new AuthorizationViewModel { ReturnUrl = returnUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
 
        }
        #endregion
        #region Авторизация
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization(AuthorizationViewModel model)
        {
            _logger.LogInformation("Requested the Authorization Page Security post");
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
           
        }
        #endregion
        #region Выход
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("Requested the Logout Page Security post");
            try
            {
                // удаляем аутентификационные куки
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
    
        }
        #endregion
    }
}
