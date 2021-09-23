using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.BLL.Mapper;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using System;
using Students.MVC.Helpers;
using AutoMapper;
using Students.DAL.Enum;

namespace Students.MVC.Controllers
{
    public class ManagerController : Controller
    {

        private readonly IManagerService _managerService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public ManagerController(IMapper mapper, UserManager<ApplicationUser> userManager,IManagerService managerService)
        {
            _managerService = managerService;
            _userManager = userManager;
            _mapper = mapper;
        }
        #region Отображения менеджеров
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersManager serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = serachParameter;
            return View(_mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
        }
        #endregion
        #region Отображения подробностей о менеджере 
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int id)
        {
            return View(_mapper.Map<ManagerViewModel>(await _managerService.GetAsync(id)));
        }
        #endregion
        #region Отображения регистрации менеджера
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region Регистрация менеджера
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(ManagerViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "manager");
                    var manager = _mapper.Map<Manager>(model);
                    manager.UserId = user.Id;
                    await _managerService.CreateAsync(manager);
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
        #region Отображения редактирования менеджера
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(_mapper.Map<ManagerViewModel>(await _managerService.GetAsync(id)));
        }
        #endregion
        #region Редактирования менеджера
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(ManagerViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _managerService.Update(_mapper.Map<Manager>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region Удаления менеджера
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            await _managerService.DeleteAsync(Id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
