using Microsoft.AspNetCore.Mvc;
using Moq;
using Students.BLL.Services;
using Students.DAL.Tests.Classes;
using Students.MVC.Controllers;
using Xunit;
using AutoFixture;
using Students.DAL.Models;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Students.DAL.Enum;
using System.Collections.Generic;

namespace Students.DAL.Tests.ControllerTests
{
    public class StudentControllerTests
    {
        public StudentsController StudentsController;
        public Mock<IUserService> UserMock { get; } = new Mock<IUserService>();
        public Mock<IStudentService> StudentServiceMock { get; } = new Mock<IStudentService>();
        public Mock<ICourseService> CourseServiceMock { get; } = new Mock<ICourseService>();
        public Mock<IGroupService> GroupServiceMock { get; } = new Mock<IGroupService>();
        public Mock<IMapper> MapperMock { get; } = new Mock<IMapper>();
        public Fixture Fixture { get; set; } = new();

        public StudentControllerTests()
        {
          //var roleManagerMock = FakeRoleManager.GetRoleManagerMock<IdentityRole>().Object; 
            var fakeUser = new FakeUserManager();
            var signInManager = new FakeSignInManager();
            StudentsController = new StudentsController(MapperMock.Object, UserMock.Object,fakeUser, signInManager,
            StudentServiceMock.Object,GroupServiceMock.Object);
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


    }
} 
    