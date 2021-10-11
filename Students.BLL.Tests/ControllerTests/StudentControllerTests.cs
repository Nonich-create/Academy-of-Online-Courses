using Moq;
using Students.MVC.Controllers;
using Xunit;
using AutoFixture;
using Students.DAL.Models;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Students.BLL.Interface;
using Microsoft.AspNetCore.Identity;
using Students.DAL.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Students.DAL.Tests.ControllerTests
{
    public class StudentControllerTests
    {
        public StudentsController StudentsController;
        private readonly IUserService _userService;
        public Mock<IStudentService> StudentServiceMock { get; } = new Mock<IStudentService>();
        public Mock<ICourseService> CourseServiceMock { get; } = new Mock<ICourseService>();
        public Mock<IGroupService> GroupServiceMock { get; } = new Mock<IGroupService>();
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public Fixture Fixture { get; set; } = new();
        private readonly SignInManager<ApplicationUser> _signInManager;

        public StudentControllerTests()
        {
            //var roleManagerMock = FakeRoleManager.GetRoleManagerMock<IdentityRole>().Object; 
            //  var fakeUser = new FakeUserManager();
            //    var signInManager = new FakeSignInManager();
            StudentsController = new StudentsController(_mapper, _userService, _userManager, _signInManager, _studentService);
             Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
             Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task Get_Student_List()
        {
            // Arrange
            var students = Fixture.CreateMany<Student>(31);

            //Act 
            var result = await StudentsController.Index("",EnumParametersStudent.None);

            //Assert
            Assert.IsType<ViewResult>(result);
            //Assert
            // _mockProductRepository.Verify(x => x.GetProductListAsync(), Times.Once);
        }


    }
} 
    