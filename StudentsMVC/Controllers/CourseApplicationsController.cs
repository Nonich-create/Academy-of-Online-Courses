using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Students.MVC.ViewModels;
using Students.BLL.Interface;
using AutoMapper;
using Students.DAL.Enum;
using Students.MVC.Models;

namespace Students.MVC.Controllers
{
    public class CourseApplicationsController : Controller
    {
        private readonly ICourseApplicationService _courseApplicationService;
        private readonly IMapper _mapper;

        public CourseApplicationsController(IMapper mapper, ICourseApplicationService courseApplicationService)
        {
            _courseApplicationService = courseApplicationService;
            _mapper = mapper;
        }

        #region Отображения заявок
        [Authorize(Roles = "admin,manager")]
        public async Task<IActionResult> Index(string searchString, EnumParametersCourseApplication searchParameter, int page = 1)
        {
            var count = await _courseApplicationService.GetCount(searchString, (EnumSearchParameters)(int)searchParameter);
            var model = _mapper.Map<IEnumerable<CourseApplicationViewModel>>((await _courseApplicationService.IndexView(searchString, (EnumSearchParameters)(int)searchParameter, page, 10)));
            var paginationModel = new PaginationModel<CourseApplicationViewModel>(count, page)
            {
                SearchString = searchString,
                SearchParameter = (int)searchParameter,
                Data = model
            };
            return View(paginationModel);
        }
        #endregion
        #region Зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Enroll(int courseApplicationId)
        {
                await _courseApplicationService.Enroll(courseApplicationId);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion
        #region Отмена зачисления студента в группу
        [HttpPost]
        [Authorize(Roles = "manager,admin,student")]
        public async Task<IActionResult> Cancel(int courseApplicationId)
        {
                await _courseApplicationService.Cancel(courseApplicationId);
                return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Отмена заявки студента
        [HttpPost]
        [Authorize(Roles = "manager,admin,student")]
        public async Task<IActionResult> CancelApplication(int courseApplicationId)
        {
            await _courseApplicationService.CancelApplication(courseApplicationId);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion

        #region Открыть заявку студента
        [HttpPost]
        [Authorize(Roles = "admin,student")]
        public async Task<IActionResult> Open(int courseApplicationId)
        {
            await _courseApplicationService.Open(courseApplicationId);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        #endregion


        public IActionResult ReturnByUrl(string ReturnUrl)
        {
            return RedirectPermanent($"~{ReturnUrl}");
        }
    }
}
