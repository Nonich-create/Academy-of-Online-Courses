using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Linq;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;
using Students.BLL.Interface;

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
        public async Task<IActionResult> Index(string searchString, EnumParametersLesson searchParameter, int page = 1)
        {
            var count = await _lessonService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<LessonViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }

        #endregion
        #region Отображения уроков определенного курса
        [Authorize(Roles = "admin,manager,teacher")] // исправить
        public async Task<IActionResult> IndexСourseId(string searchString, EnumParametersStudent searchParameter, int page = 1)
        {
            var count = await _lessonService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<LessonViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
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
            var model = new LessonViewModel
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
