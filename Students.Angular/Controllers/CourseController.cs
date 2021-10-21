using Microsoft.AspNetCore.Mvc;
using Students.BLL.Interface;

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

     //  [HttpGet]
     //  public IEnumerable<CourseDto> Get()
     //  {
     //    //  return _mapper.Map<CourseDto, Course>(_service.GetAllAsync().Result.ToList());
     //  }
    }
}
