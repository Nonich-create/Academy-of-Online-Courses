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
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupService _groupService;
        private readonly ICourseService _courseService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IApplicationCourseService _applicationCourse;

        public StudentsController(IApplicationCourseService applicationCourse, IUserService userService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IStudentService studentService, IGroupService groupService, ICourseService courseService)
        {
            _userService = userService;
            _studentService = studentService;
            _userManager = userManager;
            _groupService = groupService;
            _courseService = courseService;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _applicationCourse = applicationCourse;
        }
        #region Отображения студентов
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index()
        {
            List<Student> students = await _studentService.GetAllAsync();
            List<StudentViewModel> models = new();
            StudentViewModel model;
            foreach (var student in students)
            {
                model = Mapper.ConvertViewModel<StudentViewModel, Student>(student);

                if (student.GroupId != null)
                {
                    GroupViewModel groups = Mapper.ConvertViewModel<GroupViewModel, Group>(await _groupService.GetAsync(student.GroupId));
                    groups.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(groups.CourseId));
                    model.Group = groups;
                }

                models.Add(model);
            }
            return View(models);
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
            model = Mapper.ConvertViewModel<StudentViewModel, Student>(student);
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            return View(model);
        }
        #endregion
        #region Отображения регистрации студента
        public IActionResult Create()
        {
            return View();
        }
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
                    var student = Mapper.ConvertViewModel<Student, StudentViewModel>(model);
                    student.UserId = user.Id;
                    await _studentService.CreateAsync(student);
                    await _studentService.Save();
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
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            List<GroupViewModel> groups = Mapper.ConvertListViewModel<GroupViewModel, Group>(await _groupService.GetAllAsync());
            var model = Mapper.ConvertViewModel<EditStudentViewModel, Student>(student);
            model.Groups = groups;
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
                var student = Mapper.ConvertViewModel<Student, EditStudentViewModel>(model);
                student.GroupId = model.GroupId;
                try
                {
                    await _studentService.Update(student);
                    await _studentService.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _studentService.ExistsAsync(student.StudentId))
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
        #region Удаления студента
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> DeleteConfirmed(int StudentId)
        {
            var student = await _studentService.GetAsync(StudentId);
            if (student == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(student.UserId);
            await _applicationCourse.DeleteAsyncAll(StudentId);
            await _studentService.DeleteAsync(StudentId);
            await _userManager.DeleteAsync(user);
            await _studentService.Save();
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
    }
}
