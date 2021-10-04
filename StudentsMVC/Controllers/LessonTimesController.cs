using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Students.BLL.Mapper;
using Students.BLL.Services;
using Students.DAL.Enum;
using Students.DAL.Models;
using Students.MVC.ViewModels;

namespace Students.MVC.Controllers
{
    public class LessonTimesController : Controller
    {


        private readonly ILessonTimesService _lessonTimesService;
        private readonly ILessonService _lessonService;
        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;
        public LessonTimesController(IMapper mapper, ILessonTimesService lessonTimesService, ILessonService lessonService, IGroupService groupService)
        {
            _lessonService = lessonService;
            _lessonTimesService = lessonTimesService;
            _groupService = groupService;
            _mapper = mapper;
        }

        #region Отображения расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index(
            string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumParametersLessonTimes serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            var model = _mapper.Map<IEnumerable<LessonTimesViewModel>>((
                await _lessonTimesService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip)));
            return View(model);
        }
        #endregion
        #region Отображения добавления расписания занятий
        public async Task<IActionResult> Create()
        {
            LessonTimesViewModel model = new()
            {
                Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).ToList().Where(g => g.GroupStatus == EnumGroupStatus.Training)),
            };
            return View(model);
        }
        #endregion Отображения добавления расписания занятий
        #region Добавления расписания занятий
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(LessonTimesViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _lessonTimesService.CreateAsync(_mapper.Map<LessonTimes>(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region частичное представление уроков
        public async Task<ActionResult> GetLesson(int id)
        {
            var lessons = _mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.GetAllAsync()).OrderBy(l => l.NumberLesson));
            return PartialView(lessons.Where(l => l.CourseId == id).ToList());
        }
        #endregion
        #region Отображения редактирования расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var model = _mapper.Map<LessonTimesViewModel>(await _lessonTimesService.GetAsync(id));
            model.Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).ToList().Where(g => g.GroupStatus == EnumGroupStatus.Training));
            return View(model);
        }
        #endregion
        #region Редактирования расписания занятий
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(LessonTimesViewModel model)
        {
            if (ModelState.IsValid)
            {
                    await _lessonTimesService.Update(_mapper.Map<LessonTimes>(model));
            }
            return View(model);
        }
        #endregion

    }
}
