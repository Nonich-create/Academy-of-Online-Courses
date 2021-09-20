using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Linq;
using System.Collections.Generic;
using Students.BLL.Services;
using Students.BLL.Mapper;
using Microsoft.AspNetCore.Authorization;
using Students.DAL.Enum;
using Microsoft.AspNetCore.Identity;
using Students.MVC.Helpers;
using System;

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
        public async Task<IActionResult> Index(string sortRecords, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortRecords;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortRecords) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortRecords == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var groups = await _groupService.GetAllAsync();
            List<GroupViewModel> GroupViewModels = new();
            foreach (var group in groups)
            {
                GroupViewModels.Add(group.GroupToGroupViewModelMapping(await _managerService.GetAsync(group.ManagerId), await _teacherService.GetAsync(group.TeacherId), await _courseService.GetAsync(group.CourseId)));
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                GroupViewModels = GroupViewModels.FindAll(g => g.NumberGroup.Contains(searchString)
              //  || g.Course.Name.Contains(searchString)
              // || g.Manager.GetFullName.Contains(searchString)
              //  || g.Teacher.GetFullName.Contains(searchString)
                );
            }
           switch (sortRecords)
           {
               case "name_desc":
                   GroupViewModels = GroupViewModels.OrderByDescending(g => g.NumberGroup).ToList();
                   break;
               default:
                   GroupViewModels = GroupViewModels.OrderBy(g => g.NumberGroup).ToList();
                   break;
           }
            return View(GroupViewModels);//PaginatedList<GroupViewModel>.Create(GroupViewModels, pageNumber ?? 1, 10));
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
                var groups = (await _groupService.GetAllAsync()).Where(g => g.TeacherId == teacher.Where(t => t.UserId == id).First().Id);
                List<GroupViewModel> models = new();
                foreach (var group in groups)
                {
                    models.Add(group.GroupToGroupViewModelMapping(await _managerService.GetAsync(group.ManagerId), await _teacherService.GetAsync(group.TeacherId), await _courseService.GetAsync(group.CourseId)));
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
            List<StudentViewModel> studentViewModels = new();
            foreach (var student in (await _studentService.GetAllAsync()).Where(s => s.GroupId == id))
            {
                studentViewModels.Add(Mapper.ConvertViewModel<StudentViewModel, Student>(student));
            }
            DetailGroupViewModel model = group.GroupToDetailGroupViewModelMapping(await _managerService.GetAsync(group.ManagerId), await _teacherService.GetAsync(group.TeacherId), await _courseService.GetAsync(group.CourseId));
            model.StudentsViewModels = studentViewModels;
            return View(model);
        }
        #endregion
        #region отображения добавления группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create() => View(ExtensionMethods.GroupToGroupViewModelMapping(new Group(), (await _managerService.GetAllAsync()).ToList(), (await _teacherService.GetAllAsync()).ToList(), (await _courseService.GetAllAsync()).ToList()));
        
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
                group.GroupStatus = EnumGroupStatus.Набор;
                await _groupService.CreateAsync(group);
                return RedirectToAction("Index");
            }
     
            model = ExtensionMethods.GroupToGroupViewModelMapping(null, (await _managerService.GetAllAsync()).ToList(), (await _teacherService.GetAllAsync()).ToList(), (await _courseService.GetAllAsync()).ToList());
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
            var model = group.GroupToGroupViewModelMapping((await _managerService.GetAllAsync()).ToList(), (await _teacherService.GetAllAsync()).ToList(), (await _courseService.GetAllAsync()).ToList());
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _groupService.ExistsAsync(group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
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
