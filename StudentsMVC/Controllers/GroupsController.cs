﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Linq;
using System.Collections.Generic;
using Students.BLL.Services;
using Students.BLL.Mapper;
using Microsoft.AspNetCore.Authorization;
using Students.DAL.Enum;
using Microsoft.AspNetCore.Identity;
using Students.MVC.Helpers;
using System;
using AutoMapper;

namespace Students.MVC.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IManagerService _managerService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public GroupsController(IMapper mapper,UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentService studentService, IGroupService groupService, IManagerService managerService, ITeacherService teacherService, ICourseService courseService)
        {
            _studentService = studentService;
            _groupService = groupService;
            _managerService = managerService;
            _teacherService = teacherService;
            _courseService = courseService;
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
        }
        #region отображения групп
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersGroup serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = serachParameter;
            return View(_mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
        }
        #endregion

        #region отображения групп преподователя
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> IndexTeacher()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var idTeacher = (await _teacherService.GetAllAsync()).First(t => t.UserId == _userManager.GetUserId(User)).Id;
                var groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).AsQueryable()
                    .Where(g => g.TeacherId == idTeacher));
                return View(groups);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region отображения детали группы
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var group = _mapper.Map<DetailGroupViewModel>(await _groupService.GetAsync(id));
            group.Students = _mapper.Map<IEnumerable<StudentViewModel>>((await _studentService.GetAllAsync()).AsQueryable().Where(s => s.GroupId == group.Id));
            return View(group);
        }
        #endregion
        #region отображения добавления группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create()
        {
            GroupViewModel group = new()
            {
                Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync())),
                Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync())),
                Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync())),
            };
           return View(group);
        }
        
        #endregion
        #region добавления группы
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _groupService.CreateAsync(_mapper.Map<Group>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region Отображения редактирования группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var group = _mapper.Map<GroupViewModel>(await _groupService.GetAsync(id));
            group.Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync()));
            group.Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync()));
            group.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()));
            return View(group);
        }
        #endregion
        #region Редактирования группы
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _groupService.Update(_mapper.Map<Group>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region удаления группы
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int GroupId)
        {
            await _groupService.DeleteAsync(GroupId);
            return RedirectToAction("Index");
        }
        #endregion

        #region Перевод группы режим обучение 
        public async Task<IActionResult> StartGroup(int id)
        {
            await _groupService.StartGroup(id);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
