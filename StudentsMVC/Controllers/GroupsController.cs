using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using System.Linq;
using System.Collections.Generic;
using Students.BLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Students.DAL.Enum;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Students.MVC.Models;

namespace Students.MVC.Controllers
{
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly IManagerService _managerService;
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GroupsController(IMapper mapper, UserManager<ApplicationUser> userManager, IStudentService studentService, IGroupService groupService, IManagerService managerService, ITeacherService teacherService, ICourseService courseService)
        {
            _studentService = studentService;
            _groupService = groupService;
            _managerService = managerService;
            _teacherService = teacherService;
            _courseService = courseService;
            _userManager = userManager;
            _mapper = mapper;
        }

        #region отображения групп
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string searchString, EnumParametersGroup searchParameter, int page = 1)
        {
            var count = await _groupService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<GroupViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion

        #region отображения групп преподователя
        [Authorize(Roles = "teacher")]  // проверить
        public async Task<IActionResult> IndexTeacher(int page = 1)
        {
            var idTeacher = (await _teacherService.SearchAsync($"UserId = \"{_userManager.GetUserId(User)}\"")).Id;
            var model = _mapper.Map<IEnumerable<GroupViewModel>>((await _groupService.IndexView(idTeacher.ToString(), EnumSearchParameters.TeacherId, page, 10)));
            return View(model);
        }
        #endregion

        #region отображения детали группы
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var group = _mapper.Map<DetailGroupViewModel>(await _groupService.GetAsync(id));
            group.Students = _mapper.Map<IEnumerable<StudentViewModel>>((await _studentService.GetAllAsync()).AsQueryable().Where(s => s.GroupId == group.Id));
            group.ReturnUrl = Url;
            return View(group);
        }
        #endregion
        #region отображения добавления группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create()
        {
            GroupViewModel group = new()
            {
                Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync())),
                Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync())),
                Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync())),
            };
            return View(group);
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
                await _groupService.CreateAsync(_mapper.Map<Group>(model));
                return RedirectToAction("Index");
            }
            model.Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync()));
            model.Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync()));
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()));
            return View(model);
        }
        #endregion
        #region Отображения редактирования группы
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var group = _mapper.Map<GroupViewModel>(await _groupService.GetAsync(id));
            group.Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync()));
            group.Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync()));
            group.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()));
            return View(group);
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
                await _groupService.Update(_mapper.Map<Group>(model));
                return RedirectToAction("Index");
            }
            model.Manageres = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.GetAllAsync()));
            model.Teachers = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.GetAllAsync()));
            model.Courses = _mapper.Map<IEnumerable<CourseViewModel>>((await _courseService.GetAllAsync()));
            return View(model);
        }
        #endregion
        #region удаления группы
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int GroupId)
        {
            await _groupService.DeleteAsync(GroupId);
            return RedirectToAction("Index");
        }
        #endregion

        #region Перевод группы режим обучение 
        public async Task<IActionResult> StartGroup(int id)
        {
            await _groupService.StartGroup(id);
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
