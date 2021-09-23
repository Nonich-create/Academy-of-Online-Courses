using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Students.BLL.Services;
using Students.DAL.Tests.Classes;
using Students.MVC.Controllers;
using System;
using Xunit;
using AutoFixture;
using AutoFixture.Xunit2;
using Students.DAL.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Students.DAL.Tests.ControllerTests
{
    public class StudentControllerTests
    {
        public StudentsController StudentsController;
        public Mock<IUserService> UserMock { get; } = new Mock<IUserService>();
        public Mock<IStudentService> StudentServiceMock { get; } = new Mock<IStudentService>();
        public Mock<ICourseService> CourseServiceMock { get; } = new Mock<ICourseService>();
        public Mock<IGroupService> GroupServiceMock { get; } = new Mock<IGroupService>();
        public Fixture Fixture { get; set; } = new();

        public StudentControllerTests()
        {
          //var roleManagerMock = FakeRoleManager.GetRoleManagerMock<IdentityRole>().Object; 
          //var fakeUser = new FakeUserManager();
          //var signInManager = new FakeSignInManager();
            //StudentsController = new StudentsController(UserMock.Object,fakeUser, signInManager, roleManagerMock,
            //StudentServiceMock.Object,GroupServiceMock.Object,CourseServiceMock.Object);
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        // [Theory, AutoData]
      //[Fact]
      //public async Task Index_ActionExecutes_ReturnsViewForIndexAsync()
      //{
      //
      //    // Arrange
      ////    var students = Fixture.CreateMany<Student>(4).ToList();
      ////    StudentServiceMock.Setup(x => x.GetAllAsync())
      ////        .ReturnsAsync(students);
      ////    // Act
      //   // var result = await StudentsController.Index();  
      //
      //    // Assert
      //  //  Assert.IsType<ViewResult>(result);
      //}

      

    }
}
    