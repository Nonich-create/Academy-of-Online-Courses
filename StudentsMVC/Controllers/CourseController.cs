using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Services;
using System.Collections.Generic;
using Students.BLL.Classes;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System;

namespace Students.MVC.Controllers
{
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IApplicationCourseService _applicationCourseService;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly IManagerService _managerService;
        private readonly ITeacherService _teacherService;

        public CourseController(ITeacherService teacherService, IManagerService managerService, IGroupService groupService, ICourseService courseService, IApplicationCourseService applicationCourseService, IStudentService studentService)
        {
            _teacherService = teacherService;
            _managerService = managerService;
            _courseService = courseService;
            _applicationCourseService = applicationCourseService;
            _studentService = studentService;
            _groupService = groupService;
        }

        #region отображения курсов
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseService.GetAllAsync();
            List<CourseViewModel> models = new();
            CourseViewModel model;
            foreach (var cours in courses)
            {
                model = Mapper.ConvertViewModel<CourseViewModel, Course>(cours);
                models.Add(model);
            }
            return View(models);
        }
        #endregion
        #region отображения деталей курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var groups = await _groupService.GetAllAsync();
            List<GroupViewModel> modelsGroups = new();
            GroupViewModel modelGroup;
            foreach (var group in groups.Where(g => g.CourseId == id))
            {
                modelGroup = Mapper.ConvertViewModel<GroupViewModel, Group>(group);
                modelGroup.Manager = Mapper.ConvertViewModel<ManagerViewModel, Manager>(await _managerService.GetAsync(group.ManagerId));
                modelGroup.Teacher = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAsync(group.TeacherId));
                modelsGroups.Add(modelGroup);
            }

            var model = Mapper.ConvertViewModel<DetalisCourseViewModel, Course>(course);
            var applicationCourses = await _applicationCourseService.GetAllAsync();
            List<ApplicationCourseViewModel> ApplicationModels = new();
            ApplicationCourseViewModel ApplicationModel;
            foreach (var item in applicationCourses.Where(a => a.CourseId == id))
            {
                ApplicationModel = Mapper.ConvertViewModel<ApplicationCourseViewModel, ApplicationCourse>(item);
                ApplicationModel.Student = Mapper.ConvertViewModel<StudentViewModel, Student>(await _studentService.GetAsync(item.StudentId));
                ApplicationModels.Add(ApplicationModel);
            }
            model.Groups = modelsGroups;
            model.ApplicationCourseViewModels = ApplicationModels;
            return View(model);
        }
        #endregion
        #region отображения создания курса
        public IActionResult Create()
        {
            return View();
        }
        #endregion
        #region создания курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = Mapper.ConvertViewModel<Course, CourseCreateViewModel>(model);
                course.Price = (decimal)model.Price;
                await _courseService.CreateAsync(course);
                await _courseService.Save();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            return View(model);
        }
        #endregion
        #region отображения редактирование курса
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var model = Mapper.ConvertViewModel<CourseViewModel, Course>(course);
            return View(model);
        }
        #endregion
        #region редактирование курса
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(CourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var course = Mapper.ConvertViewModel<Course, CourseViewModel>(model);
                await _courseService.Update(course);
                await _courseService.Save();
            }
            return View(model);
        }
        #endregion
        #region отображения удаление курса
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseService.GetAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        #endregion
        #region удаление курса
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int CourseId)
        {
            var course = await _courseService.GetAsync(CourseId);
            if (course == null)
            {
                return NotFound();
            }
            await _courseService.DeleteAsync(CourseId);
            await _courseService.Save();
            return RedirectToAction("Index");
        }
        #endregion
        #region Отображения добавления урока в выбранный курс
        [Authorize(Roles = "admin,manager")]
        public IActionResult CreateLesson(int id)
        {
            if (id == 0)
            {
                return View();
            }
            LessonViewModel model = new();
            model.CourseId = id;
            return View(model);
        }
        #endregion
    }
}
