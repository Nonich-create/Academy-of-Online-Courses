using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.DAL.Models;
using Students.BLL.Interface;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;

namespace Students.MVC.Controllers
{
    public class ManagerController : Controller
    {

        private readonly IManagerService _managerService;
        private readonly IGroupService _groupService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public ManagerController(IMapper mapper, UserManager<ApplicationUser> userManager, IManagerService managerService, IGroupService groupService)
        {
            _managerService = managerService;
            _groupService = groupService;
            _userManager = userManager;
            _mapper = mapper;
        }
        #region Отображения менеджеров
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(string searchString, EnumParametersManager searchParameter, int page = 1)
        {
            var count = await _managerService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<ManagerViewModel>>((await _managerService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<ManagerViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion

        #region Отображения подробностей о менеджере 
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Details(int id, string Url)
        {
            var model = _mapper.Map<ManagerViewModel>(await _managerService.GetAsync(id));
            var groups = await _groupService.SearchAllAsync($"ManagerId == {id}");
            model.ReturnUrl = Url;
            model.Groups = _mapper.Map<IEnumerable<GroupViewModel>>(groups);
            return View(model);
        }
        #endregion

        #region Отображения дополнительной информации о преподователе 
        [Authorize(Roles = "manager")]
        [ActionName("DetailsManager")]
        public async Task<IActionResult> Details()
        {
            var model = _mapper.Map<ManagerViewModel>(await _managerService.SearchAsync($"UserId = \"{_userManager.GetUserId(User)}\""));
            var groups = await _groupService.SearchAllAsync($"ManagerId == {model.Id}");
            model.Groups = _mapper.Map<IEnumerable<GroupViewModel>>(groups);
            return View(model);
        }
        #endregion

        #region Отображения регистрации менеджера
        [Authorize(Roles = "admin")]
        public IActionResult Create() => View();
        #endregion
        #region Регистрация менеджера
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(ManagerViewModel model)
        {
            if (ModelState.IsValid)
            {
                    ApplicationUser user = new() { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    var manager = _mapper.Map<Manager>(model);
                    manager.UserId = user.Id;
                    await _managerService.CreateAsync(manager,user,model.Password);
                    return RedirectToAction("Index");
            }
            return View(model);
        }
        #endregion
        #region Отображения редактирования менеджера
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(int id, string Url)
        {
            var model = _mapper.Map<EditPersonViewModel>(await _managerService.GetAsync(id));
            model.ReturnUrl = Url;
            return View(model);
        }
        #endregion
        #region Редактирования менеджера
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Edit(EditPersonViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _managerService.Update(_mapper.Map<Manager>(model));
                return RedirectPermanent($"~{model.ReturnUrl}");
            }
            return View(model);
        }
        #endregion
        #region Удаления менеджера
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            await _managerService.DeleteAsync(Id);
            return RedirectToAction("Index");
        }
        #endregion
        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
