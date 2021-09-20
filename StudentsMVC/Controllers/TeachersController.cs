using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using System.Collections.Generic;
using Students.BLL.Mapper;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;
using Students.MVC.Helpers;

namespace Students.MVC.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TeachersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ITeacherService teacherService)
        {
            _userManager = userManager;
            _teacherService = teacherService;
            _roleManager = roleManager;
        }
        #region Отображения преподователей
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortRecords;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortRecords) ? "name_desc" : "";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var teachers = await _teacherService.GetAllAsync();
            List<TeacherViewModel> TeacherViewModels = new();
            foreach (var teacher in teachers)
            {
                TeacherViewModels.Add(Mapper.ConvertViewModel<TeacherViewModel, Teacher>(teacher));
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                TeacherViewModels = TeacherViewModels.FindAll(c => c.GetFullName.Contains(searchString));
            }
            switch (sortRecords)
            {
                case "name_desc":
                    TeacherViewModels = TeacherViewModels.OrderByDescending(t => t.GetFullName).ToList();
                    break;
                default:
                    TeacherViewModels = TeacherViewModels.OrderBy(s => s.GetFullName).ToList();
                    break;
            }
            return View(TeacherViewModels);// PaginatedList<TeacherViewModel>.Create(TeacherViewModels, pageNumber ?? 1, 10));
        }
        #endregion
        #region Отображения дополнительной информации о преподователях
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id)
        {
            var teacher = await _teacherService.GetAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            TeacherViewModel model = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(teacher);
            return View(model);
        }
        #endregion
        #region Отображения регистрации преподователя
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region Регистрация преподователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "teacher");
                    var teacher = Mapper.ConvertViewModel<Teacher, TeacherViewModel>(model);
                    teacher.UserId = user.Id;
                    await _teacherService.CreateAsync(teacher);
                    await _teacherService.Save();
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }
            }
            return View(model);
        }
        #endregion
        #region Отображения редактирования преподователя
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var teacher = await _teacherService.GetAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            var model = Mapper.ConvertViewModel<EditTeacherViewModel, Teacher>(teacher);
            return View(model);
        }
        #endregion
        #region Редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(EditTeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                var teacher = Mapper.ConvertViewModel<Teacher, EditTeacherViewModel>(model);
                try
                {
                    await _teacherService.Update(teacher);
                    await _teacherService.Save();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _teacherService.ExistsAsync(teacher.Id))
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
        #endregion

        #region Удаления преподователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int TeacherId)
        {
            var teacher = await _teacherService.GetAsync(TeacherId);
            if (teacher == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            if (teacher != null)
            {
                await _teacherService.DeleteAsync(TeacherId);
                await _userManager.DeleteAsync(user);
                await _teacherService.Save();
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
