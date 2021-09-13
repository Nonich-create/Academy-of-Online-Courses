using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Students.BLL.Classes;
using Students.MVC.Models;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Students.MVC.Controllers
{
    [ActivatorUtilitiesConstructorAttribute]
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(ICourseService courseService, IStudentService studentService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _courseService = courseService;
            _studentService = studentService;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #region Отображения витрины с курсами
        public async Task<IActionResult> Index()
        {
                List<Course> courses = await _courseService.GetAllAsync();
                if (courses is null)
                {
                    return null;
                }
                List<CourseViewModel> models = new();
                CourseViewModel model;
                foreach (var course in courses)
                {
                    model = Mapper.ConvertViewModel<CourseViewModel, Course>(course);
                    models.Add(model);
                }
                return View(models); ;      
        }
        #endregion
        #region Вызов ошибки
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
        #region Подробнее о курсе
        public async Task<IActionResult> Detailed(int id)
        {
                var course = await _courseService.GetAsync(id);
                if (course == null)
                {
                    return NotFound();
                }
                var model = new CourseViewModel()
                {
                    Name = course.Name,
                    Description = course.Description,
                    Duration = course.Duration,
                    CourseId = course.CourseId,
                    Price = course.Price
                };
                return View(model);
        }
        #endregion
        #region Оставления заявки на курс
        [HttpPost]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> PutRequest(int CourseId)
        {
                if (_signInManager.IsSignedIn(User))
                {     
                    var id = _userManager.GetUserId(User);
                    var student = await _studentService.GetAllAsync(); 
                    await _studentService.PutRequest(student.Where(s => s.UserId == id).First().StudentId, CourseId);
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                return RedirectToAction("Authorization", "Security");
                }
        }
          #endregion
    }
}
