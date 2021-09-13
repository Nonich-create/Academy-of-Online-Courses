using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Students.BLL.Classes;
using Students.BLL.Services;
using Students.DAL.Models;
using Students.MVC.ViewModels;

namespace Students.MVC.Controllers
{
    public class LessonPlansController : Controller
    {
 

        private readonly ILessonPlanService _lessonPlanService;
        private readonly ILessonService _lessonService;
        private readonly IGroupService _groupService;

        public LessonPlansController(ILessonPlanService lessonPlanService, ILessonService lessonService, IGroupService groupService)
        {
            _lessonService = lessonService;
            _lessonPlanService = lessonPlanService;
            _groupService = groupService;
        }

        #region Отображения расписания занятий
        [Authorize(Roles = "admin,manager,teacher")]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var lessonPlans = await _lessonPlanService.GetAllAsync();
            List<LessonPlanViewModel> models = new();
            LessonPlanViewModel model;
            foreach (var item in lessonPlans)
            {
                model = Mapper.ConvertViewModel<LessonPlanViewModel, LessonPlan>(item);
                model.Group = Mapper.ConvertViewModel<GroupViewModel, Group>(await _groupService.GetAsync(item.GroupId));
                model.Lesson = Mapper.ConvertViewModel<LessonViewModel, Lesson>(await _lessonService.GetAsync((int)item.LessonId));
                models.Add(model);
            }
            return View("Index", models);
        }
        #endregion

        public async Task<IActionResult> Create()
        {
            LessonPlanViewModel model = new()
            {
                Lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>(await _lessonService.GetAllAsync()),
                Groups = Mapper.ConvertListViewModel< GroupViewModel, Group>(await _groupService.GetAllAsync()),
            };
            var Lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>(await _lessonService.GetAllAsync());
            var Groups = Mapper.ConvertListViewModel<GroupViewModel, Group>(await _groupService.GetAllAsync());
            SelectList groups = new SelectList(Groups, "GroupId", "NumberGroup");
            ViewBag.Groups = groups;
       
            SelectList lessons = new SelectList(Lessons.Where(l => l.CourseId == 1), "LessonId", "Name"); ;
            ViewBag.Lessons = lessons;
            return View();
        }

        public async Task<ActionResult> GetLesson(int id)
        {
            var lessons = Mapper.ConvertListViewModel<LessonViewModel, Lesson>(await _lessonService.GetAllAsync());
            return PartialView(lessons.Where(l => l.CourseId == id).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LessonPlanId,DateOfTheLesson,LessonId,GroupId")] LessonPlan lessonPlan)
        {
     
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LessonPlanId,DateOfTheLesson,LessonId,GroupId")] LessonPlan lessonPlan)
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
