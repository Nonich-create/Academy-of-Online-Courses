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
using System.Collections.Generic;

namespace Students.BLL.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly StudentService _studentService;
        public Fixture Fixture { get; set; } = new();
        private readonly UnitOfWork UnitOfWork;
        private readonly Mock<ILogger<Student>> _mockLogger;

        public StudentServiceTests()
        {
            var myDatabaseName = "StudentsMVCBD";
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: myDatabaseName)
                .Options;
            UnitOfWork = new UnitOfWork(new Context(options));
            _mockLogger = new Mock<ILogger<Student>>();
            _studentService = new StudentService(UnitOfWork, _mockLogger.Object);

           // Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
           // Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }




        [Fact]
        public async Task Get_Paginated_List()
        {
            // Arrange
             var students = Fixture.Build<Student>().Without(s => s.Id).CreateMany(20);
             await UnitOfWork.StudentRepository.AddRangeAsync(students);
             await UnitOfWork.SaveAsync();
            //  var studentsId = (await UnitOfWork.StudentRepository.GetAllAsync()).Select(s => s.Id);
            // foreach(var id in studentsId)
            // {
            //    var assessment = Fixture.Build<Assessment>().With(s => s.StudentId, id).Without(a => a.Id).Create();
            //    var ttt =  await UnitOfWork.AssessmentRepository.AddAsync(assessment);
            // }
            // await UnitOfWork.SaveAsync();
            // var assessments = await UnitOfWork.AssessmentRepository.GetAllAsync();

            //Act 
            var result = await _studentService.IndexView("",EnumSearchParameters.None,1, 10);

            //Assert 
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task Search_ReturnsViewResultNull()
        {
            // Arrange
            var students = Fixture.Build<Student>().Without(s => s.Id).CreateMany(20);
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            await UnitOfWork.SaveAsync();
            int currentPage = 1;
            int pageSize = 10;
            string searchString = "";
      
            //Act
            var result = await _studentService.SearchAllAsync(currentPage, pageSize,searchString, EnumSearchParameters.Surname);
      
            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchName_ReturnsViewResult()
        {
            // Arrange
            var students = Fixture.Build<Student>().Without(s => s.Id).CreateMany(20);
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            await UnitOfWork.SaveAsync();
            int currentPage = 1;
            int pageSize = 10;
            students = await UnitOfWork.StudentRepository.GetAllAsync();
            string searchString = students.ToList()[4].Name;
          
            //Act
            var result = await _studentService.SearchAllAsync(currentPage, pageSize,searchString, EnumSearchParameters.Name);
          
            //Assert
            Assert.NotEmpty(result);
        }

    }
}
