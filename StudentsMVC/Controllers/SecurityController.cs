using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Threading.Tasks;
using Students.BLL.Interface;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
namespace Students.MVC.Controllers
{
    public class SecurityController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public SecurityController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        #region Отображения редактирования пользователя
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Security(string id, string Url)
        {
                var user = _mapper.Map<UserViewModel>(await _userService.GetAsync(id));
                user.ReturnUrl = Url;
                return View(user);
        }
        #endregion
        #region Редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Security(UserViewModel model)
        {
                if (ModelState.IsValid)
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(model.Id);
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.NormalizedUserName = model.Email.ToUpper();
                    user.NormalizedEmail = model.Email.ToUpper();
                    user.PhoneNumber = model.PhoneNumber;
                    await _userService.Update(user);
                }
                return View(model);
        }
        #endregion
        #region Отображения смены пароля пользователя
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> ChangePassword(string id, string Url)
        {
                var user = await _userService.GetAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var model = new ChangePasswordViewModel()
                {
                    UserIdentityId = user.Id,
                    ReturnUrl = Url
                };
                return View(model);
        }
        #endregion
        #region Смена пароля
        [HttpPost]
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByIdAsync(model.UserIdentityId);
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
        #endregion
        #region Отображения авторизация 
        [HttpGet]
        public  IActionResult Authorization(string returnUrl = null)
        {
                return View(new AuthorizationViewModel { ReturnUrl = returnUrl });
        }
        #endregion
        #region Авторизация
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Authorization(AuthorizationViewModel model)
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
        #endregion
        #region Выход
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
    
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
