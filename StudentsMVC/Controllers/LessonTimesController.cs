using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Students.BLL.Mapper;
using Students.BLL.Services;
using Students.DAL.Models;
using Students.MVC.ViewModels;

namespace Students.MVC.Controllers
{
    public class LessonTimesController : Controller
    {
 

        private readonly ILessonTimesService _lessonTimesService;
        private readonly ILessonService _lessonService;
        private readonly IGroupService _groupService;

        public LessonTimesController(ILessonTimesService lessonTimesService, ILessonService lessonService, IGroupService groupService)
        {
            _lessonService = lessonService;
            _lessonTimesService = lessonTimesService;
            _groupService = groupService;
        }

        #region Отображения расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var lessonTimes = await _lessonTimesService.GetAllAsync();
            List<LessonTimesViewModel> models = new();
            LessonTimesViewModel model;
            foreach (var item in lessonTimes)
            {
                model = Mapper.ConvertViewModel<LessonTimesViewModel, LessonTimes>(item);
                model.Group = Mapper.ConvertViewModel<GroupViewModel, Group>(await _groupService.GetAsync(item.GroupId));
                model.Lesson = Mapper.ConvertViewModel<LessonViewModel, Lesson>(await _lessonService.GetAsync((int)item.LessonId));
                models.Add(model);
            }
            return View("Index", models);
        }
        #endregion

        public async Task<IActionResult> Create()
        {
            LessonTimesViewModel model = new()
            {
                Lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>((await _lessonService.GetAllAsync()).ToList()),
                Groups = Mapper.ConvertListViewModel< GroupViewModel, Group>((await _groupService.GetAllAsync()).ToList()),
            };
            var Lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>((await _lessonService.GetAllAsync()).ToList());
            var Groups = Mapper.ConvertListViewModel<GroupViewModel, Group>((await _groupService.GetAllAsync()).ToList());
            SelectList groups = new SelectList(Groups, "GroupId", "NumberGroup");
            ViewBag.Groups = groups;
       
            SelectList lessons = new SelectList(Lessons.Where(l => l.CourseId == 1), "LessonId", "Name"); ;
            ViewBag.Lessons = lessons;
            return View();
        }

        public async Task<ActionResult> GetLesson(int id)
        {
            var lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>((await _lessonService.GetAllAsync()).ToList());
            return PartialView(lessons.Where(l => l.CourseId == id).ToList());
        }

    
    }
}
