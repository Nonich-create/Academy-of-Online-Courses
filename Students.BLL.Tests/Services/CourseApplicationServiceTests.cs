using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Students.BLL.DataAccess;
using Students.BLL.Services;
using Students.DAL.Models;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Students.DAL.Enum;
using Students.DAL.Specifications;

namespace Students.BLL.Tests.Services
{
    public class CourseApplicationServiceTests
    {
        private readonly CourseApplicationService _courseApplicationService;
        public Fixture Fixture { get; set; } = new();
        private readonly UnitOfWork UnitOfWork;
        private readonly Mock<ILogger<CourseApplication>> _mockLogger;

        public CourseApplicationServiceTests()
        {
            var myDatabaseName = "StudentsMVCBD";
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: myDatabaseName)
                .Options;
            UnitOfWork = new UnitOfWork(new Context(options));
            _mockLogger = new Mock<ILogger<CourseApplication>>();
            _courseApplicationService = new CourseApplicationService(UnitOfWork, _mockLogger.Object);
        }

        [Fact]
        public async Task Enroll_courseApplication_GetNull()
        {
            //Arrange
            var courseApplications = Fixture.Build<CourseApplication>().Without(c => c.Id).CreateMany<CourseApplication>(20);
            var students = Fixture.CreateMany<Student>(20);
            await UnitOfWork.CourseApplicationRepository.AddRangeAsync(courseApplications);
            await UnitOfWork.SaveAsync();

            //Act 
            var result = await _courseApplicationService.GetAsync(0);

            //Assert 
            Assert.Null(result);
        }

        [Fact]
        public async Task Enroll_student_GetNotNullIdGroup()
        {
            //Arrange
            var group = Fixture.Build<Group>().With(g => g.Id, 1).Create();
            var student = Fixture.Build<Student>().With(s => s.Id,1).With(s => s.GroupId, group.Id).Create();
            var courseApplication = Fixture.Build<CourseApplication>().With(c => c.Id,1).With(c => c.StudentId, student.Id).Create();
            await UnitOfWork.GroupRepository.AddAsync(group);
            await UnitOfWork.StudentRepository.AddAsync(student);
            await UnitOfWork.CourseApplicationRepository.AddAsync(courseApplication);
            await UnitOfWork.SaveAsync();
            var specStudent = new StudentWithItemsSpecifications(courseApplication.StudentId);
   
            //Act 
            var result = await UnitOfWork.StudentRepository.GetAsync(specStudent, false);

            //Assert 
            Assert.NotNull(result.GroupId);
        }

 
    }
}


    