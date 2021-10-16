using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Students.BLL.Interface;
using Students.DAL.Enum;
using Students.DAL.Models;
using Students.MVC.Models;
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
        public async Task<IActionResult> Index(string searchString, EnumParametersLessonTimes searchParameter, int page = 1)
        {
            var count = await _lessonTimesService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<LessonTimesViewModel>>((await _lessonTimesService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<LessonTimesViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion

        #region Отображения расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("IndexGroupId")]
        public async Task<IActionResult> IndexGroupId(int groupId, int page = 1)
        {
            var count = await _lessonTimesService.GetCount(groupId);
            var model = _mapper.Map<IEnumerable<LessonTimesViewModel>>((await _lessonTimesService.IndexView(groupId, page, 10)));
            var paginationModel = new PaginationModel<LessonTimesViewModel>(count, page)
            {
                Data = model
            };
            return View(paginationModel);
        }
        #endregion

        #region Отображения добавления расписания занятий
        public async Task<IActionResult> Create(string Url)
        {
            LessonTimesViewModel model = new()
            {
                Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).ToList().Where(g => g.GroupStatus == EnumGroupStatus.Training)),
                ReturnUrl = Url
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
                return RedirectPermanent($"~{model.ReturnUrl}");
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
        #region Отображения редактирования расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> EditByIdLessonTimes(int id, string Url)
        {
            var model = _mapper.Map<LessonTimesViewModel>(await _lessonTimesService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion
        #region Редактирования расписания занятий
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> EditByIdLessonTimes(LessonTimesViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _lessonTimesService.Update(_mapper.Map<LessonTimes>(model));
                return RedirectPermanent($"~{model.ReturnUrl}");
            }
            return View(model);
        }
        #endregion

        #region Удаления расписания урока
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Delete(int Id, string Url)
        {
            await _lessonTimesService.DeleteAsync(Id);
            return RedirectPermanent($"~{Url}");
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
