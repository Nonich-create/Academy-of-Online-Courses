using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Students.BLL.DataAccess;
using Students.DAL.Models;
using Students.BLL.Services;
using Students.DAL.Tests.Classes;
using Students.MVC.Helpers;
using Students.MVC.Controllers;
using Students.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using AutoFixture.Xunit2;
using AutoFixture;
using System.Linq;
using AutoMapper;
using Students.BLL.Interface;

namespace Students.DAL.Tests.ControllerTests
{
    public class HomeControllerTests 
    {
        public HomeController HomeController { get; set; }
        public Mock<IStudentService> StudentServiceMock { get; } = new Mock<IStudentService>();
        public Mock<ICourseService> CourseServiceMock { get; } = new Mock<ICourseService>();
        public Fixture Fixture { get; set; } = new();
        private readonly IMapper _mapper;
        public HomeControllerTests()
        {
            var fakeUser = new FakeUserManager();
            var signInManager = new FakeSignInManager();
          //  HomeController = new HomeController(_mapper, CourseServiceMock.Object, StudentServiceMock.Object,
          //      fakeUser, signInManager);
          //  Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
          //  Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public void Index_searchString()
        {
            // Arrange
            var courses = Fixture.CreateMany<Course>(4).ToList();

          //  CourseServiceMock.Setup(c => c.)
         //   StudentServiceMock.Setup(s => s.PutRequest(students[1].Id, courses[1].Id));

            // Act
            var result = HomeController.PutRequest(courses[1].Id);

            //Assert
            Assert.IsType<ViewResult>(result);
        }
        //[Theory, AutoData]
        //[Fact]
        //public async Task Index_ActionExecutes_ReturnsViewForIndexAsync( )
        //{
        //    // Arrange
        //    var courses = Fixture.CreateMany<Course>(4).ToList();
        //    CourseServiceMock.Setup(x => x.GetAllAsync())
        //        .ReturnsAsync(courses);
        //
        //    // Act
        //    // var result =  await HomeController.();  
        //
        //    // Assert
        //  //  Assert.IsType<ViewResult>(result);
        //}

        [Fact]
        public void PutRequest_()
        {
            // Arrange
            var courses = Fixture.CreateMany<Course>(4).ToList();
            var users = Fixture.CreateMany<ApplicationUser>(4).ToList();
            var students = Fixture.CreateMany<Student>(4).ToList();
            for(int i = 0;i < students.Count;i++)
            {
                students[i].UserId = users[i].Id;
            }
            StudentServiceMock.Setup(s => s.PutRequest(students[1].Id, courses[1].Id));
        
            // Act
            var result = HomeController.PutRequest(courses[1].Id);
        
            //Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
