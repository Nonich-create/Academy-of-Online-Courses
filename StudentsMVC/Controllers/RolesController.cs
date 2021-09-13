using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Students.MVC.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #region Отображения ролей
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            _logger.LogInformation("Requested the Index Page Role");
            try
            {
                return View(_roleManager.Roles.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }

        }
        #endregion
        #region Отображения регистрации роли
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Requested the Create Page Role");
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
        }
        #endregion
        #region Регистрация роли
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(string name)
        {
            _logger.LogInformation("Requested the Create Page Role post");
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return View(name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
             
        }
        #endregion
        #region Удаление роли
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(string id)
        {
            _logger.LogInformation("Requested the Delete Page Role post");
            try
            {
                IdentityRole role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    _ = await _roleManager.DeleteAsync(role);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
 
        }
        #endregion
        #region Отображения пользователей 
        [Authorize(Roles = "admin")]
        public IActionResult UserList()
        {
            _logger.LogInformation("Requested the Index Page Role");
            try
            {
                return View(_userManager.Users.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Caught");
                return View();
            }
 
        }
        #endregion
        #region Отображения редактирование пользователя
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId)
        {
            _logger.LogInformation("Requested the Edit Page Role");
            try
            {
                // получаем пользователя
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // получем список ролей пользователя
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var allRoles = _roleManager.Roles.ToList();
                    ChangeRoleViewModel model = new()
                    {
                        UserId = user.Id,
                        UserEmail = user.Email,
                        UserRoles = userRoles,
                        AllRoles = allRoles
                    };
                    return View(model);
                }

                return NotFound();
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
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            _logger.LogInformation("Requested the Edit Page Role post");
            try
            {
                // получаем пользователя
                ApplicationUser user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    // получем список ролей пользователя
                    var userRoles = await _userManager.GetRolesAsync(user);
                    // получаем все роли
                    _ = _roleManager.Roles.ToList();
                    // получаем список ролей, которые были добавлены
                    var addedRoles = roles.Except(userRoles);
                    // получаем роли, которые были удалены
                    var removedRoles = userRoles.Except(roles);

                    await _userManager.AddToRolesAsync(user, addedRoles);

                    await _userManager.RemoveFromRolesAsync(user, removedRoles);

                    return RedirectToAction("UserList");
                }

                return NotFound();
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
