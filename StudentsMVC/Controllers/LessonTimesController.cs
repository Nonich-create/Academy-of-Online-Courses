using System;
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
        public LessonTimesController(IMapper mapper,ILessonTimesService lessonTimesService, ILessonService lessonService, IGroupService groupService)
        {
            _lessonService = lessonService;
            _lessonTimesService = lessonTimesService;
            _groupService = groupService;
            _mapper = mapper;
        }

        #region Отображения расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index(string sortRecords, string searchString, int skip, int take, EnumPageActions action, EnumSearchParametersLessonTimes serachParameter)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = serachParameter;
            return View(_mapper.Map<IEnumerable<LessonTimesViewModel>>((await _lessonTimesService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip))));
        }
        #endregion

        public async Task<IActionResult> Create()
        {
            LessonTimesViewModel model = new();
            var Lessons = _mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.GetAllAsync()).ToList());
            var Groups = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.GetAllAsync()).ToList());
            model.Lessons = Lessons.ToList();
            model.Groups = Groups.ToList();
           SelectList groups = new(Groups, "GroupId", "NumberGroup");
            ViewBag.Groups = groups;
       
            SelectList lessons = new(Lessons.Where(l => l.CourseId == 1), "LessonId", "Name"); ;
            ViewBag.Lessons = lessons;
            return View();
        }

        public async Task<ActionResult> GetLesson(int id)
        {
            var lessons = _mapper.Map<IEnumerable<LessonViewModel>>((await _lessonService.GetAllAsync()));
            return PartialView(lessons.Where(l => l.CourseId == id).ToList());
        }

    
    }
}
