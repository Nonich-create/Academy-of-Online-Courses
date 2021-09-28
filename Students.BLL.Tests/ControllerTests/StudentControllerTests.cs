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
using AutoMapper;
using Students.DAL.Enum;
using System.Collections.Generic;
using Students.MVC.Mapper;

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
        private readonly IMapper _mapper;
        public Fixture Fixture { get; set; } = new();
        private static readonly IMapper Mapper =
          new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())).CreateMapper();
        public StudentControllerTests()
        {
          //var roleManagerMock = FakeRoleManager.GetRoleManagerMock<IdentityRole>().Object; 
            var fakeUser = new FakeUserManager();
            var signInManager = new FakeSignInManager();
            StudentsController = new StudentsController(Mapper, UserMock.Object,fakeUser, signInManager,
            StudentServiceMock.Object,GroupServiceMock.Object);
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
         
        //[Theory,AutoData]
        [Fact]
        public async Task Index_ActionExecutes_ReturnsViewResultSearch_WithStudents()
        {
            // Arrange
               string searchString = "adsdasd";
               const int take = 10;
               var students = Fixture.CreateMany<Student>(31).ToList();
               StudentServiceMock.Setup(s => s.DisplayingIndex(EnumPageActions.notActions, students[2].Surname, EnumSearchParameters.Surname, take, 0))
                .ReturnsAsync(students);
            // Act
            var result = await StudentsController.Index(null, students[2].Surname, 0,take, EnumPageActions.notActions, EnumSearchParametersStudent.Surname);
            //  Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultSearchStudents = students.Where(s => s.Surname == students[2].Surname).ToList();
            var model = Assert.IsType<List<Student>>(viewResult.ViewData.Model);
            Assert.Equal(resultSearchStudents.Count, model.Count);
            //Assert.IsType<ViewResult>(result);
        }
    }
} 
    