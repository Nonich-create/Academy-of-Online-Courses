using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Students.Angular.Dto;
using Students.BLL.Interface;
using Students.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Students.Angular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoursesController : ControllerBase
    {


        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

         [HttpGet]  
         public async Task<IEnumerable<CourseDto>> GetAsync()
         {
             return _mapper.Map<IEnumerable<CourseDto>>(await _courseService.GetAllAsync());
        }
    }
}
