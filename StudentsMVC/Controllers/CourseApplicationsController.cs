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
using AutoMapper;
using Students.DAL.Enum;

namespace Students.MVC.Controllers
{
    public class CourseApplicationsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly ICourseApplicationService _courseApplicationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CourseApplicationsController(IMapper mapper,UserManager<ApplicationUser> userManager, ICourseService courseService, IStudentService studentService, ICourseApplicationService courseApplicationService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _userManager = userManager;
            _courseApplicationService = courseApplicationService;
            _mapper = mapper;
        }

        #region Отображения заявок
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string searchString, EnumParametersStudent serachParameter, int page = 1)
        {
            ViewData["searchString"] = searchString;
            ViewData["serachParameter"] = (int)serachParameter;
            var count = await _courseApplicationService.GetCount(searchString, (EnumSearchParameters)(int)serachParameter);
            var model = _mapper.Map<IEnumerable<CourseApplicationViewModel>>((await _courseApplicationService.IndexView(searchString, (EnumSearchParameters)(int)serachParameter, page, 10)));
            return View(model);
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
