using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Linq;
using System.Collections.Generic;
using Students.BLL.Services;
using Students.BLL.Classes;
using Microsoft.AspNetCore.Authorization;
using Students.BLL.Enum;
using System;
using Microsoft.AspNetCore.Identity;

namespace Students.MVC.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IManagerService _managerService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IStudentService studentService, IGroupService groupService, IManagerService managerService, ITeacherService teacherService, ICourseService courseService)
        {
            _studentService = studentService;
            _groupService = groupService;
            _managerService = managerService;
            _teacherService = teacherService;
            _courseService = courseService;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        #region отображения групп
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Index()
        {
            var groups = await _groupService.GetAllAsync();
            List<GroupViewModel> models = new();
            GroupViewModel model;
            foreach (var group in groups)
            {
                model = Mapper.ConvertViewModel<GroupViewModel, Group>(group);
                model.Manager = Mapper.ConvertViewModel<ManagerViewModel, Manager>(await _managerService.GetAsync(group.ManagerId));
                model.Teacher = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAsync(group.TeacherId));
                model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(group.CourseId));
                models.Add(model);
            }
            return View(models);
        }
        #endregion


        #region отображения групп преподователя
        [Authorize(Roles = "teacher")]
        public async Task<IActionResult> IndexTeacher()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var id = _userManager.GetUserId(User);
                var teacher = await _teacherService.GetAllAsync();
                var groups = await _groupService.GetAllAsync();
                groups = groups.Where(g => g.TeacherId == teacher.Where(t => t.UserId == id).First().TeacherId).ToList();
                List<GroupViewModel> models = new();
                GroupViewModel model;
                foreach (var group in groups)
                {
                    model = Mapper.ConvertViewModel<GroupViewModel, Group>(group);
                    model.Manager = Mapper.ConvertViewModel<ManagerViewModel, Manager>(await _managerService.GetAsync(group.ManagerId));
                    model.Teacher = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAsync(group.TeacherId));
                    model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(group.CourseId));
                    models.Add(model);
                }
                return View(models);
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region отображения детали группы
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id)
        {
            var group = await _groupService.GetAsync(id);

            if (group == null)
            {
                return NotFound();
            }
            var students = await _studentService.GetAllAsync();
            List<StudentViewModel> modelsstudents = new();
            StudentViewModel modelStudent;
            foreach (var student in students.Where(s => s.GroupId == id))
            {
                modelStudent = Mapper.ConvertViewModel<StudentViewModel, Student>(student);
                modelsstudents.Add(modelStudent);
            }
            DetalisGroupViewModel model;
            model = Mapper.ConvertViewModel<DetalisGroupViewModel, Group>(group);
            model.Manager = Mapper.ConvertViewModel<ManagerViewModel, Manager>(await _managerService.GetAsync(group.ManagerId));
            model.Teacher = Mapper.ConvertViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAsync(group.TeacherId));
            model.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(group.CourseId));
            model.StudentsViewModels = modelsstudents;
            return View(model);
        }
        #endregion
        #region отображения добавления группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create()
        {
            GroupViewModel model = new()
            {
                Manageres = Mapper.ConvertListViewModel<ManagerViewModel, Manager>(await _managerService.GetAllAsync()),
                Teachers = Mapper.ConvertListViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAllAsync()),
                Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>(await _courseService.GetAllAsync())
            };

            return View(model);
        }
        #endregion
        #region добавления группы
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = Mapper.ConvertViewModel<Group, GroupViewModel>(model);
                group.GroupStatus = EnumGroupStatus.Набор.ToString();
                await _groupService.CreateAsync(group);
                await _groupService.Save();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            model = new()
            {
                Manageres = Mapper.ConvertListViewModel<ManagerViewModel, Manager>(await _managerService.GetAllAsync()),
                Teachers = Mapper.ConvertListViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAllAsync()),
                Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>(await _courseService.GetAllAsync())
            };
            return View(model);
        }
        #endregion
        #region Отображения редактирования группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _groupService.GetAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            var model = Mapper.ConvertViewModel<GroupViewModel, Group>(group);
            model.Manageres = Mapper.ConvertListViewModel<ManagerViewModel, Manager>(await _managerService.GetAllAsync());
            model.Teachers = Mapper.ConvertListViewModel<TeacherViewModel, Teacher>(await _teacherService.GetAllAsync());
            model.Courses = Mapper.ConvertListViewModel<CourseViewModel, Course>(await _courseService.GetAllAsync());
            return View(model);
        }
        #endregion
        #region Редактирования группы
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = Mapper.ConvertViewModel<Group, GroupViewModel>(model);
                try
                {

                    await _groupService.Update(group);
                    await _groupService.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _groupService.ExistsAsync(group.GroupId))
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
        #region удаления группы
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int GroupId)
        {
            var group = await _groupService.GetAsync(GroupId);
            if (group == null)
            {
                return NotFound();
            }
            await _groupService.DeleteAsync(GroupId);
            await _groupService.Save();
            return RedirectToAction("Index");
        }
        #endregion

        #region Перевод группы режим обучение 
        public async Task<IActionResult> StartGroup(int id)
        {
            await _groupService.StartGroup(id);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
    }
}
