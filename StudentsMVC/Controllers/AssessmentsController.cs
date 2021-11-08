using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using AutoMapper;
using Students.BLL.Interface;

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
            var students = await _studentService.GetAsync(id);
            var assessments = _mapper.Map<IEnumerable<AssessmentViewModel>>(await _assessmentService.GetAllAsync(students.Id,students.Group.CourseId));
            return View(assessments);
        }
        #endregion
        #region отображения оценок студентов
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(int GroupId) 
        {
            var assessments = _mapper.Map<IEnumerable<AssessmentViewModel>>(await _assessmentService.SearchAllAsync($"Student.GroupId == {GroupId}"));
            return View(assessments);
        }
        #endregion
        #region отображения добавления оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Create(int idAssessment, string Url)
        {
            var assessment = await _assessmentService.GetAsync(idAssessment);
            var model = _mapper.Map<AssessmentViewModel>(assessment);
            model.ReturnUrl = Url;
            return View(model);
        }
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
                return RedirectPermanent($"~{model.ReturnUrl}");
            }
            var modelValidate = _mapper.Map<AssessmentViewModel>(await _assessmentService.GetAsync(model.Id));
            modelValidate.ReturnUrl = model.ReturnUrl;
            return View(modelValidate);
        }
        #endregion
        #region отображения редактирование оценки
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int idAssessment, string Url) 
        {
            var assessment = await _assessmentService.GetAsync(idAssessment);
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
            var modelValidate = _mapper.Map<AssessmentViewModel>(await _assessmentService.GetAsync(model.Id));
            modelValidate.ReturnUrl = model.ReturnUrl;
            return View(modelValidate);
        }
        #endregion

        #region удаление оценки
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _assessmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        #endregion

    

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
