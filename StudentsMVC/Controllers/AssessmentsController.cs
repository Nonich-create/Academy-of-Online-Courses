using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using AutoMapper;
using Students.DAL.Enum;

namespace Students.MVC.Controllers
{
    public class AssessmentsController : Controller // Добавить дату создания заявки
    {

        private readonly ICourseService _courseService;
        private readonly ILessonService _lessonService;
        private readonly IStudentService _studentService;
        private readonly IAssessmentService _assessmentService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public AssessmentsController(ITeacherService teacherService, IGroupService groupService, IAssessmentService assessmentService, ICourseService courseService, ILessonService lessonService, IStudentService studentService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _teacherService = teacherService;
            _groupService = groupService;
            _courseService = courseService;
            _lessonService = lessonService;
            _studentService = studentService;
            _assessmentService = assessmentService;
            _userManager = userManager;
            _mapper = mapper;
        }

        #region отображения оценок студента
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> StudentAssessments()
        {
            var id = _userManager.GetUserId(User);
            var students = (await _studentService.GetAllAsync()).First(s => s.UserId == _userManager.GetUserId(User));
            var assessments = await _assessmentService.GetAssessmentsByStudentId(students.Id);
            var assessmentViewModels = _mapper.Map<IEnumerable<AssessmentViewModel>>((await _assessmentService.GetAssessmentsByStudentId(students.Id)).ToList());
            return View(assessmentViewModels);
        }
        #endregion
        #region отображения оценок студентов
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> Index(int GroupId,int take = 9999) 
        {
            var assessments = (await _assessmentService.SearchAllAsync(
                $"{GroupId}", EnumSearchParameters.Student_GroupId, EnumPageActions.NotActions, take,0)).OrderBy(a => a.Lesson.NumberLesson);
            return View(_mapper.Map<IEnumerable<AssessmentViewModel>>(assessments));
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
                var assessment = _mapper.Map<Assessment>(model);
                await _assessmentService.CreateAsync(assessment);
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(model);
        }
        #endregion
        #region отображения редактирование оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int idAssessment, string Url)
        {
            var assessment = await _assessmentService.SearchAsync($"Id = {idAssessment}");
            var model = _mapper.Map<AssessmentViewModel>(assessment);
            model.ReturnUrl = Url;
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
                var assessment = _mapper.Map<Assessment>(model);
                await _assessmentService.Update(assessment);
                return RedirectPermanent($"~{model.ReturnUrl}");
            }
            var modelValidate = _mapper.Map<AssessmentViewModel>(await _assessmentService.SearchAsync($"Id = {model.Id}"));
            modelValidate.ReturnUrl = model.ReturnUrl;
            return View(modelValidate);
        }
        #endregion

        public IActionResult ReturnByUrl(AssessmentViewModel model)
        {
            return RedirectPermanent($"~{model.ReturnUrl}");
        }

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
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
