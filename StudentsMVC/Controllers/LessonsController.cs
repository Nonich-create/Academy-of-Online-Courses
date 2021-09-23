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
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersLesson serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = serachParameter;
            return View(_mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
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
                model = _mapper.Map<LessonViewModel>(lesson);
                model.Course = _mapper.Map<CourseViewModel>(await _courseService.GetAsync(lesson.CourseId));
                LessonViewModels.Add(model);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                LessonViewModels = LessonViewModels.FindAll(l => l.Name.Contains(searchString)
                || l.Course.Name.Contains(searchString));
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
            model = _mapper.Map<LessonViewModel>(lesson);
            model.Course = _mapper.Map<CourseViewModel>(await _courseService.GetAsync(lesson.CourseId));
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
            LessonViewModel model = new()
            {
                Courses = _mapper.Map<IEnumerable<CourseViewModel>>(await _courseService.GetAllAsync())
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
                var lesson = _mapper.Map<Lesson>(model);
                await _lessonService.CreateAsync(lesson);
                return RedirectPermanent("~/Lessons/Index");
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
            if (lesson == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<LessonViewModel>(lesson);
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()).ToList());
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
                try
                {
                    await _lessonService.Update(lesson);
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
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()).ToList());
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
            return RedirectToAction("Index");
        }
        #endregion
    }
}
