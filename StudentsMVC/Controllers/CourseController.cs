﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseApplicationService _applicationCourseService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public CourseController(IMapper mapper,IGroupService groupService, ICourseService courseService, ICourseApplicationService applicationCourseService)
        {
            _courseService = courseService;
            _applicationCourseService = applicationCourseService;
            _groupService = groupService;
            _mapper = mapper;
        }

        #region отображения курсов
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersCourse serachParameter)
        {
            ViewData["searchString"] = searchString;
            //ViewData["CurrentSort"] = sortRecords;
            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortRecords) ? "name_desc" : "";
            //ViewData["DateSortParm"] = sortRecords == "Date" ? "date_desc" : "Date";
            //switch (sortRecords)
            //{
            //    case "name_desc":
            //        courses = courses.OrderByDescending(c => c.Name).ToList();
            //        break;
            //    default:
            //        courses = courses.OrderBy(c => c.Name).ToList();
            //        break;
            //}
            return View(_mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
        }
        #endregion
        #region отображения деталей курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id)
        {
            var course = _mapper.Map<DetalisCourseViewModel>(await _courseService.GetAsync(id));
            course.Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).AsQueryable().Where(g => g.CourseId == id)).ToList();
            course.CourseApplications = _mapper.Map<IEnumerable<CourseApplicationViewModel>>((await _applicationCourseService.GetAllAsync()).AsQueryable().Where(c => c.CourseId == id)).ToList();
            return View(course);
        }
        #endregion
        #region отображения создания курса
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region создания курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
               if (ModelState.IsValid)
               {
                var course = _mapper.Map<Course>(model);
                await _courseService.CreateAsync(course);
            return RedirectToAction("Index");
               }
               return View(model);
        }
        #endregion
        #region отображения редактирование курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            return View(_mapper.Map<CourseViewModel>(await _courseService.GetAsync(id)));
        }
        #endregion
        #region редактирование курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _courseService.Update(_mapper.Map<Course>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region отображения удаление курса
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        #endregion
        #region удаление курса
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int CourseId)
        {
            await _courseService.DeleteAsync(CourseId);
            return RedirectToAction("Index");
        }
        #endregion
        #region Отображения добавления урока в выбранный курс
        [Authorize(Roles = "admin,manager")]
        public IActionResult CreateLesson(int id)
        {
            if (id == 0)
            {
                return View();
            }
            LessonViewModel model = new();
            model.CourseId = id;
            return View(model);
        }
        #endregion
    }
}
