using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.BLL.Classes;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;

namespace Students.MVC.Controllers
{
    public class AssessmentsController : Controller
    {

        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IStudentService _studentService;
        private readonly IAssessmentService _assessmentService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AssessmentsController(ITeacherService teacherService, IGroupService groupService, IAssessmentService assessmentService, ICourseService courseService, ILessonService lessonService, IStudentService studentService, UserManager<ApplicationUser> userManager)
        {
            _teacherService = teacherService;
            _groupService = groupService;
            _courseService = courseService;
            _lessonService = lessonService;
            _studentService = studentService;
            _assessmentService = assessmentService;
            _userManager = userManager;
        }

        #region отображения оценок студента
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> StudentAssessments()
        {
            var id = _userManager.GetUserId(User);
            var students = await _studentService.GetAllAsync();
            var studentId = students.Single(s => s.UserId == id).StudentId;
            var assessments = await _assessmentService.GetAssessmentsByStudentId(studentId);
            var assessmentViewModels = Mapper.ConvertListViewModel<AssessmentViewModel, Assessment>(assessments);
            return View(assessmentViewModels);
        }
        #endregion
        #region отображения оценок студентов
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> Index(int groupId)
        {
            var teachers = await _teacherService.GetAllAsync();
            var teacher = teachers.First(t => t.UserId == _userManager.GetUserId(User));
            var groups = await _groupService.GetAllAsync();
            groups = groups.Where(g => g.TeacherId == teacher.PersonId && g.GroupId == groupId).ToList();
            var assessments = await _assessmentService.GetAllAsync();

            List<AssessmentViewModel> assessmentViewModels = new();
            AssessmentViewModel assessmentViewModel;
            assessmentViewModel = await MapList(assessments, assessmentViewModels);

            List<AssessmentViewModel> assessmentViewModelsSorted = new();

            foreach (var group in groups)
            {
                assessmentViewModelsSorted.AddRange(assessmentViewModels.Where(a => a.Student.GroupId == group.GroupId).ToList());
            }

            return View(assessmentViewModelsSorted);
        }

        private async Task<AssessmentViewModel> MapList(List<Assessment> assessments, List<AssessmentViewModel> assessmentViewModels)
        {
            AssessmentViewModel assessmentViewModel;
            foreach (var assessment in assessments)
            {
                assessmentViewModel = Mapper.ConvertViewModel<AssessmentViewModel, Assessment>(assessment);
                assessmentViewModel.Student = Mapper.ConvertViewModel<StudentViewModel, Student>(await _studentService.GetAsync(assessment.StudentId));
                assessmentViewModel.Lesson = Mapper.ConvertViewModel<LessonViewModel, Lesson>(await _lessonService.GetAsync(assessment.LessonId));
                assessmentViewModels.Add(assessmentViewModel);
            }

            return assessmentViewModel;
        }
        #endregion
        #region отображения деталей о оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var assessment = await _assessmentService.GetAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            var model = Mapper.ConvertViewModel<AssessmentViewModel, Assessment>(assessment);
            return View(model);
        }
        #endregion
        #region отображения добавления оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public IActionResult Create() => View();
        #endregion
        #region добавления оценки
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Create(AssessmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var assessment = Mapper.ConvertViewModel<Assessment, AssessmentViewModel>(model);
                await _assessmentService.CreateAsync(assessment);
                await _assessmentService.Save();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(model);
        }
        #endregion
        #region отображения редактирование оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var assessment = await _assessmentService.GetAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            var model = Mapper.ConvertViewModel<AssessmentViewModel, Assessment>(assessment);
            return View(model);
        }
        #endregion
        #region  редактирование оценки
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(AssessmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var assessment = Mapper.ConvertViewModel<Assessment, AssessmentViewModel>(model);
                try
                {
                    await _assessmentService.Update(assessment);
                    await _assessmentService.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _assessmentService.ExistsAsync(assessment.AssessmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(model);
        }
        #endregion
        #region удаление оценки
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assessment = await _assessmentService.GetAsync(id);
            if (assessment == null)
            {
                return NotFound();
            }
            await _assessmentService.DeleteAsync(id);
            await _assessmentService.Save();
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
