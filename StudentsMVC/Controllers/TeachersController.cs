using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;

namespace Students.MVC.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public TeachersController(IMapper mapper,UserManager<ApplicationUser> userManager, ITeacherService teacherService)
        {
            _userManager = userManager;
            _teacherService = teacherService;
            _mapper = mapper;
        }

        #region Отображения преподователей
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string searchString, EnumParametersTeacher searchParameter, int page = 1)
        {
            var count = await _teacherService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<TeacherViewModel>>((await _teacherService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<TeacherViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion

        #region Отображения дополнительной информации о преподователях
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var model = _mapper.Map<TeacherViewModel>(await _teacherService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion

        #region Отображения дополнительной информации о преподователях
        [Authorize(Roles = "teacher")]
        [ActionName("DetailsTeacher")]
        public async Task<IActionResult> Details()
        {
            var model = _mapper.Map<TeacherViewModel>(await _teacherService.SearchAsync($"UserId = \"{_userManager.GetUserId(User)}\""));
            return View(model);
        }
        #endregion

        #region Отображения регистрации преподователя
        public IActionResult Create() => View();
        #endregion

        #region Регистрация преподователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Create(TeacherViewModel model) 
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "teacher");
                    var teacher = _mapper.Map<Teacher>(model);
                    teacher.UserId = user.Id;
                    await _teacherService.CreateAsync(teacher);
                    return RedirectToAction("Index");
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

        #region Отображения редактирования преподователя
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var model = _mapper.Map<PersonEditViewModel>(await _teacherService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion

        #region Редактирования пользователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager,teacher")]
        public async Task<IActionResult> Edit(PersonEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _teacherService.Update(_mapper.Map<Teacher>(model));
                return ReturnByUrl(model.ReturnUrl);
            }
            return View(model);
        }
        #endregion

        #region Удаления преподователя
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int TeacherId)
        {
            await _teacherService.DeleteAsync(TeacherId);
            return RedirectToAction("Index");
        }
        #endregion

        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
