using Microsoft.AspNetCore.Mvc;
using Students.DAL.Models;
using Students.BLL.Services;
using Students.BLL.Mapper;
using Students.MVC.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Students.BLL.Interface;
using VKCommunityWebApi.Controllers;
using Students.BLL.Repository.Base;
using Microsoft.Extensions.Logging;
using Students.DAL.Repositories.Base;

namespace StudentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAPIController : WebApiBase<Student, StudentAPIController>
    {


        public StudentAPIController(IRepository<Student> repository, ILogger<StudentAPIController> logger):base (repository, logger)
        {

        }

    }
}
