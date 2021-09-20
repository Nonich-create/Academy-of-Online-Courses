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

namespace Students.MVC.Controllers
{
    public class LessonsController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        public LessonsController(ICourseService courseService, ILessonService lessonService)
        {
            _lessonService = lessonService;
            _courseService = courseService;
        }
        #region Отображения уроков
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortRecords;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortRecords) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortRecords == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var lessonses = await _lessonService.GetAllAsync();
            List<LessonViewModel> LessonViewModels = new();
            LessonViewModel model;
            foreach (var lesson in lessonses)
            {
                model = Mapper.ConvertViewModel<LessonViewModel, Lesson>(lesson);
                model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(lesson.CourseId));
                LessonViewModels.Add(model);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                LessonViewModels = LessonViewModels.FindAll(l => l.Name.Contains(searchString)
                || l.Course.Name.Contains(searchString));
            }
            switch (sortRecords)
            {
                case "name_desc":
                    LessonViewModels = LessonViewModels.OrderByDescending(l => l.Course.Name).ToList();
                    break;
                default:
                    LessonViewModels = LessonViewModels.OrderBy(l => l.Course.Name).ToList();
                    break;
            }
            return View();//PaginatedList<LessonViewModel>.Create(LessonViewModels, pageNumber ?? 1, 10));
        }
        #endregion
        #region Отображения уроков определенного курса
        [ActionName("IndexСourseId")]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(int id, string sortRecords, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortRecords;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortRecords) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortRecords == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var lessonses = await _lessonService.GetAllAsync();
            List<LessonViewModel> LessonViewModels = new();
            LessonViewModel model;
            foreach (var lesson in lessonses.Where(l => l.CourseId == id))
            {
                model = Mapper.ConvertViewModel<LessonViewModel, Lesson>(lesson);
                model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(lesson.CourseId));
                LessonViewModels.Add(model);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                LessonViewModels = LessonViewModels.FindAll(l => l.Name.Contains(searchString)
                || l.Course.Name.Contains(searchString));
            }
            switch (sortRecords)
            {
                case "name_desc":
                    LessonViewModels = LessonViewModels.OrderByDescending(l => l.Course.Name).ToList();
                    break;
                default:
                    LessonViewModels = LessonViewModels.OrderBy(l => l.Course.Name).ToList();
                    break;
            }
            return View();//PaginatedList<LessonViewModel>.Create(LessonViewModels, pageNumber ?? 1, 10));
        }
        #endregion
        #region Отображения деталей урока
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var lesson = await _lessonService.GetAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }
            LessonViewModel model;
            model = Mapper.ConvertViewModel<LessonViewModel, Lesson>(lesson);
            model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(lesson.CourseId));
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
                var lesson = Mapper.ConvertViewModel<Lesson, LessonViewModel>(model);
                await _lessonService.CreateAsync(lesson);
                await _lessonService.Save();
                return RedirectPermanent("~/Course/Index");
            }
            return View(model);
        }
        #endregion
        #region Отображения добавления урока с выбором курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> CreateWithCourse()
        {
            LessonViewModel model = new()
            {
                Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>((await _courseService.GetAllAsync()).ToList())
            };

            return View(model);
        }
        #endregion
        #region Добавления урока с выбором курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> CreateWithCourse(LessonViewModel model)
        {
            var lessonCourse = await _lessonService.GetAllAsync();
            if (lessonCourse.Any(l => l.CourseId == model.CourseId && l.NumberLesson == model.NumberLesson))
            {
                ModelState.AddModelError("Занятие", "Занятия с таким номером в данном курсе существуют");
            }
            if (ModelState.IsValid)
            {
                var lesson = Mapper.ConvertViewModel<Lesson, LessonViewModel>(model);
                await _lessonService.CreateAsync(lesson);
                await _lessonService.Save();
                return RedirectPermanent("~/Lessons/Index");
            }
            model = new()
            {
                Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>((await _courseService.GetAllAsync()).ToList())
            };
            return View(model);
        }
        #endregion
        #region Отображения редактирования урока
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var lesson = await _lessonService.GetAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            var model = Mapper.ConvertViewModel<LessonViewModel, Lesson>(lesson);
            model.Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>((await _courseService.GetAllAsync()).ToList());
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
                var lesson = Mapper.ConvertViewModel<Lesson, LessonViewModel>(model);
                try
                {
                    await _lessonService.Update(lesson);
                    await _lessonService.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _lessonService.ExistsAsync(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectPermanent($"~{model.ReturnUrl}");
            }
            model.Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>((await _courseService.GetAllAsync()).ToList());
            return View(model);
        }
        #endregion
        #region Удаления урока
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> DeleteConfirmed(int LessonId)
        {
            var lesson = await _lessonService.GetAsync(LessonId);
            if (lesson == null)
            {
                return NotFound();
            }
            await _lessonService.DeleteAsync(LessonId);
            await _lessonService.Save();
            return RedirectToAction("Index");
        }
        #endregion
    }
}
