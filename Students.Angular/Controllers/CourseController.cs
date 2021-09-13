using Microsoft.AspNetCore.Mvc;
using Students.Angular.Classes;
using Students.Angular.Dto;
using Students.DAL.Models;
using Students.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Students.Angular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {


        private readonly ICourseService _service;

        public CoursesController(ICourseService service)
        {
            _service = service;
        }

        [HttpGet]
        public IEnumerable<CourseDto> Get()
        {
            return Mapper.ConvertListViewModel<CourseDto, Course>(_service.GetAllAsync().Result.ToList());
        }
    }
}
