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
using AutoMapper;
using Students.DAL.Enum;

namespace Students.MVC.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public TeachersController(IMapper mapper,UserManager<ApplicationUser> userManager, ITeacherService teacherService)
        {
            _userManager = userManager;
            _teacherService = teacherService;
            _mapper = mapper;
        }

        #region Отображения преподователей
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersTeacher serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            return View(_mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
        }
        #endregion

        #region Отображения дополнительной информации о преподователях
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id)
        {
            return View(_mapper.Map<TeacherViewModel>(await _teacherService.GetAsync(id)));
        }
        #endregion

        #region Отображения дополнительной информации о преподователях
        [Authorize(Roles = "teacher")]
        [ActionName("DetailsTeacher")]
        public async Task<IActionResult> Details()
        {
            var teachers = (await _teacherService.GetAllAsync()).First(t => t.UserId == _userManager.GetUserId(User)).Id;
            return View(_mapper.Map<TeacherViewModel>(await _teacherService.GetAsync(teachers)));
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
                    var teacher = _mapper.Map<Teacher>(model);
                    teacher.UserId = user.Id;
                    await _teacherService.CreateAsync(teacher);
                    return RedirectToAction("Index");
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
            return View(_mapper.Map<TeacherViewModel>(await _teacherService.GetAsync(id)));
        }
        #endregion
        #region Редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(TeacherViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _teacherService.Update(_mapper.Map<Teacher>(model));
                return RedirectToAction("Index");
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
            var user = await _userManager.FindByIdAsync(teacher.UserId);
            if (teacher != null)
            {
                await _teacherService.DeleteAsync(TeacherId);
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
