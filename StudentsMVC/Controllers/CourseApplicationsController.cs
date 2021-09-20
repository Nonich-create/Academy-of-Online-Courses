using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.BLL.Mapper;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using Microsoft.Extensions.Logging;

namespace Students.MVC.Controllers
{
    public class CourseApplicationsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ICourseApplicationService _courseApplicationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CourseApplicationsController(UserManager<ApplicationUser> userManager, ICourseService courseService, IStudentService studentService, ICourseApplicationService courseApplicationService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _userManager = userManager;
            _courseApplicationService = courseApplicationService;
        }

        #region Отображения заявок
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index()
        {
                var courseApplications = await _courseApplicationService.GetAllAsync();
                List<CourseApplicationViewModel> CourseApplicationViewModels = new();
                CourseApplicationViewModel model;
                foreach (var item in courseApplications)
                {
                    model = Mapper.ConvertViewModel<CourseApplicationViewModel, CourseApplication>(item);
                    model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(item.CourseId));
                    model.Student = Mapper.ConvertViewModel<StudentViewModel, Student>(await _studentService.GetAsync(item.StudentId));
                    CourseApplicationViewModels.Add(model);
                }
                return View(CourseApplicationViewModels);
        }
        #endregion
        #region Зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Enroll(int courseApplicationId)
        {
                var courseApplication = await _courseApplicationService.GetAsync(courseApplicationId);
                if(courseApplication == null) { return RedirectToAction(nameof(Index));}
                await _courseApplicationService.Enroll(courseApplication);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
        #region Отмена зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Cancel(int courseApplicationId)
        {
                var courseApplication = await _courseApplicationService.GetAsync(courseApplicationId);
                if (courseApplication == null) { return RedirectToAction(nameof(Index)); }
                await _courseApplicationService.Cancel(courseApplication);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
    }
}
