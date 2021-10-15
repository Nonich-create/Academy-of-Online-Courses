using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;

namespace Students.MVC.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ICourseApplicationService _applicationCourseService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public CourseController(IMapper mapper, IGroupService groupService, ICourseService courseService, ICourseApplicationService applicationCourseService)
        {
            _courseService = courseService;
            _applicationCourseService = applicationCourseService;
            _groupService = groupService;
            _mapper = mapper;
        }

        #region отображения курсов
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            var count = await _courseService.GetCount(searchString, EnumSearchParameters.Name);
            var model = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.IndexView(searchString, EnumSearchParameters.Name, page, 10)));
            var paginationModel = new PaginationModel<CourseViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)EnumSearchParameters.Name,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion
        #region отображения деталей курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var course = _mapper.Map<DetalisCourseViewModel>(await _courseService.GetAsync(id));
            course.Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.SearchAllAsync($"CourseId = {id}")));
            course.CourseApplications = _mapper.Map<IEnumerable<CourseApplicationViewModel>>((await _applicationCourseService.SearchAllAsync($"CourseId = {id}")));
            course.ReturnUrl = Url;
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
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var model = _mapper.Map<CourseViewModel>(await _courseService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
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
                return RedirectPermanent($"~{model.ReturnUrl}");
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
            LessonViewModel model = new();
            model.CourseId = id;
            return View(model);
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
