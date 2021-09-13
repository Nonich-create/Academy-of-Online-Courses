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
using Microsoft.Extensions.Logging;

namespace Students.MVC.Controllers
{
    public class ApplicationCoursesController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IApplicationCourseService _applicationCourseService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationCoursesController(UserManager<ApplicationUser> userManager, ICourseService courseService, IStudentService studentService, IApplicationCourseService applicationCourseService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _userManager = userManager;
            _applicationCourseService = applicationCourseService;
        }

        #region Отображения заявок
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index()
        {
                var applicationCourses = await _applicationCourseService.GetAllAsync();
                List<ApplicationCourseViewModel> models = new();
                ApplicationCourseViewModel model;
                foreach (var item in applicationCourses)
                {
                    model = Mapper.ConvertViewModel<ApplicationCourseViewModel, ApplicationCourse>(item);
                    model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(item.CourseId));
                    model.Student = Mapper.ConvertViewModel<StudentViewModel, Student>(await _studentService.GetAsync(item.StudentId));
                    models.Add(model);
                }
                return View(models);
        }
        #endregion
        #region Зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Enroll(int ApplicationCourseId)
        {
                var model = await _applicationCourseService.GetAsync(ApplicationCourseId);
                if(model == null) { return RedirectToAction(nameof(Index));}
                await _applicationCourseService.Enroll(model);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
        #region Отмена зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Cancel(int ApplicationCourseId)
        {
                var model = await _applicationCourseService.GetAsync(ApplicationCourseId);
                if (model == null) { return RedirectToAction(nameof(Index)); }
                await _applicationCourseService.Cancel(model);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
    }
}
