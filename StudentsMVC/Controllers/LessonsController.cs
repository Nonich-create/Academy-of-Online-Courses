using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.BLL.Mapper;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using System.Linq;
using System;
using Students.MVC.Helpers;
using AutoMapper;
using Students.DAL.Enum;

namespace Students.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IMapper _mapper;

        public LessonsController(ICourseService courseService, ILessonService lessonService, IMapper mapper)
        {
            _lessonService = lessonService;
            _courseService = courseService;
            _mapper = mapper;
        }

        #region Отображения уроков
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index(
            string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumParametersLesson serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            var model = (await _lessonService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip));
            return View(_mapper.Map<IEnumerable<LessonViewModel>>(model));
        }

        #endregion
        #region Отображения уроков определенного курса
        [ActionName("IndexСourseId")]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(int id,
            string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumParametersLesson serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            var model = (await _lessonService.DisplayingIndexByIdCourse(id,action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip));
            return View(model);
        }
        #endregion

        #region Отображения деталей урока
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var lesson = await _lessonService.GetAsync(id);
            var model = _mapper.Map<LessonViewModel>(lesson);
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion
        #region Отображения добавления урока
        [Authorize(Roles = "admin,manager")]
        public IActionResult Create(int CourseId)
        {
            return View(CourseId);
        }
        #endregion
        #region Добавления урока
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(LessonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lesson = _mapper.Map<Lesson>(model);
                await _lessonService.CreateAsync(lesson);
                return RedirectPermanent("~/Course/Index");
            }
            return View(model);
        }
        #endregion
        #region Отображения добавления урока с выбором курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> CreateWithCourse()
        {
            var model = new LessonViewModel();
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>(await _courseService.GetAllAsync());
            return View(model);
        }
        #endregion
        #region Добавления урока с выбором курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> CreateWithCourse(LessonViewModel model) 
        {
            if (await _lessonService.CheckRecord(model.CourseId, model.NumberLesson) == false)
            {
                ModelState.AddModelError("Занятие", "Занятия с таким номером в данном курсе существует");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var lesson = _mapper.Map<Lesson>(model);
                    await _lessonService.CreateAsync(lesson);
                    return RedirectPermanent("~/Lessons/Index");
                }
            }
            model = new()
            {
                Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()).ToList())
            };
            return View(model);
        }
        #endregion

        #region Отображения редактирования урока
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var lesson = await _lessonService.GetAsync(id);
            var model = _mapper.Map<LessonViewModel>(lesson);
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>(await _courseService.GetAllAsync());
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion

        #region Редактирования урока
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(LessonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var lesson = _mapper.Map<Lesson>(model);
                await _lessonService.Update(lesson);
                return ReturnByUrl(model.ReturnUrl);
            }
            return View(model);
        }
        #endregion
        #region Удаления урока
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> DeleteConfirmed(int LessonId)
        {
            await _lessonService.DeleteAsync(LessonId);
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
