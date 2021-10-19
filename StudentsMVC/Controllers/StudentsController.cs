using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Interface;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;
using System.IO;

namespace Students.MVC.Controllers
{
    public class StudentsController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly ICourseApplicationService _courseApplicationService;
        private readonly IAssessmentService _assessmentService;
        private readonly IMapper _mapper;

        public StudentsController(IMapper mapper, IAssessmentService assessmentService, ICourseApplicationService courseApplicationService, IUserService userService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentService studentService)
        {
            _userService = userService;
            _courseApplicationService = courseApplicationService;
            _studentService = studentService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _assessmentService = assessmentService;
        }

        #region Отображения студентов с использованием пагинации
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(string searchString, EnumParametersStudent searchParameter, int page = 1)
        {
            var count = await _studentService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<StudentViewModel>>((await _studentService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<StudentViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };

            return View(paginationModel);
        }
        #endregion

        [HttpGet("DownloadStudent")]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> DownloadStudent(int studentId)
        {
            Stream content = await _studentService.GetContent(studentId);
            var contentType = "text/plain";
            var fileName = $"Студент{studentId}.docx";
            return File(content, contentType, fileName);
        }

        #region Отображения подробной информации о студенте
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var student = await _studentService.GetAsync(id);
            var user = await _userService.GetAsync(student.UserId);
            var courseApplications = await _courseApplicationService.SearchAllAsync($"StudentId == {id}");
            var assessments = await _assessmentService.SearchAllAsync($"StudentId == {id}");
            DetaliStudentViewModel model = _mapper.Map<DetaliStudentViewModel>(student);
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.CourseApplications = _mapper.Map<IEnumerable<CourseApplicationViewModel>>(courseApplications);
            model.Assessments = _mapper.Map<IEnumerable<AssessmentViewModel>>(assessments);
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion

        #region Отображения подробной информации о студенте
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> DetailsStudents()
        {
            var student = _mapper.Map<DetaliStudentViewModel>(await _studentService.SearchAsync($"UserId = \"{_userManager.GetUserId(User)}\""));
            var user = await _userService.GetAsync(student.UserId);
            var courseApplications = await _courseApplicationService.SearchAllAsync($"StudentId == {student.Id}");
            var assessments = await _assessmentService.SearchAllAsync($"StudentId == {student.Id}");
            student.Email = user.Email;
            student.PhoneNumber = user.PhoneNumber;
            student.CourseApplications = _mapper.Map<IEnumerable<CourseApplicationViewModel>>(courseApplications);
            student.Assessments = _mapper.Map<IEnumerable<AssessmentViewModel>>(assessments);
            return View(student);
        }
        #endregion

        #region Отображения регистрации студента
        public IActionResult Create() => View();
        #endregion

        #region Регистрация студента
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "student");
                    var student = _mapper.Map<Student>(model);
                    student.UserId = user.Id;
                    await _studentService.CreateAsync(student);
                    await Authenticate(model.Email, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View(model);
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region Отображения редактирования студента
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var model = _mapper.Map<EditStudentViewModel>(await _studentService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion

        #region Редактирования студента
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher,student")]
        public async Task<IActionResult> Edit(EditStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = _mapper.Map<Student>(model);
                student.GroupId = model.GroupId;
                await _studentService.Update(student);
                return ReturnByUrl(model.ReturnUrl);
            }
            return View(model);
        }
        #endregion

        #region Удаления студента
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Delete(int StudentId)
        {
            await _studentService.DeleteAsync(StudentId);
            return RedirectToAction("Index");
        }
        #endregion

        #region Аутефикация студента
        [HttpPost]
        public async Task<IActionResult> Authenticate(string Email, string Password)
        {
            await _signInManager.PasswordSignInAsync(Email, Password, false, false);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }

    }
}
