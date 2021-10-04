using System.Collections.Generic;
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
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public StudentsController(IMapper mapper,IUserService userService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentService studentService, IGroupService groupService)
        {
            _userService = userService;
            _studentService = studentService;
            _userManager = userManager;
            _groupService = groupService;
            _signInManager = signInManager;
            _mapper = mapper;
        }
       
        #region Отображения студентов
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index(string sortRecords, string sortParm, string searchString, int skip, int take, EnumPageActions action, EnumParametersStudent serachParameter)
        {
            ViewData["sortRecords"] = sortRecords;
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            var model = _mapper.Map<IEnumerable<StudentViewModel>>(
                await _studentService.DisplayingIndex(action, searchString, (EnumSearchParameters)(int)serachParameter, take, skip));
            return View(model);
        }
        #endregion

        #region Отображения подробной информации о студенте
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            StudentViewModel model;
            var user = await _userService.GetAsync(student.UserId);
            model = _mapper.Map<StudentViewModel>(student);
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            return View(model);
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
        [Authorize(Roles = "admin,manager,teacher")]
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
        [Authorize(Roles = "admin,manager,teacher")]
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
