using AutoFixture;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Students.BLL.DataAccess;
using Students.BLL.Interface;
using Students.BLL.Services;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Students.DAL.Enum;

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
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task Get_Paginated_List()
        {
            // Arrange
            var students = Fixture.CreateMany<Student>(20);
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            await UnitOfWork.SaveAsync();

            //Act 
            var result = await _studentService.IndexView("",EnumSearchParameters.None,1, 10);

            //Assert 
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public async Task Search_ReturnsViewResultNull()
        {
            // Arrange
            var students = Fixture.CreateMany<Student>(20);
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
            var students = Fixture.CreateMany<Student>(10);
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            await UnitOfWork.SaveAsync();
            int currentPage = 1;
            int pageSize = 10;
            string searchString = students.ToList()[4].Name;

            //Act
            var result = await _studentService.SearchAllAsync(currentPage, pageSize,searchString, EnumSearchParameters.Name);
        
            //Assert
        
            Assert.NotEmpty(result);
        }

    }
}
