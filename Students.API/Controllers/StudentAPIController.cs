using Microsoft.AspNetCore.Mvc;
using Students.DAL.Models;
using Students.BLL.Services;
using Students.BLL.Mapper;
using Students.MVC.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;
        private readonly ICourseService _courseService;

        public StudentAPIController(IUserService userService, IStudentService studentService, IGroupService groupService, ICourseService courseService)
        {
            _studentService = studentService;
            _userService = userService;
            _groupService = groupService;
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentViewModel>>> Get()
        {
            var students = await _studentService.GetAllAsync();
            List<StudentViewModel> models = new();
            StudentViewModel model;
            foreach (var student in students)
            {
                model = Mapper.ConvertViewModel<StudentViewModel, Student>(student);

                if (student.GroupId != null)
                {
                    GroupViewModel groups = Mapper.ConvertViewModel<GroupViewModel, Group>(await _groupService.GetAsync(student.GroupId));
                    groups.Course = Mapper.ConvertViewModel<CourseViewModel, Course>(await _courseService.GetAsync(groups.CourseId));
                    model.Group = groups;
                }

                models.Add(model);
            }
            return models;
        }

        [HttpPost]
        public async Task Post(Student model)
        {
            await _studentService.CreateAsync(model);
        }


        [HttpGet]
        public async Task<StudentViewModel> GetStudent(int id)
        {
            var student = await _studentService.GetAsync(id);
            if (student == null)
            {
                return null;
            }
            StudentViewModel model;
            var user = await _userService.GetAsync(student.UserId);
            model = Mapper.ConvertViewModel<StudentViewModel, Student>(student);
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            return model;
        }
    }
}
