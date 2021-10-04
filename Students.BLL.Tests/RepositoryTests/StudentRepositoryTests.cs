using Students.BLL.DataAccess;
using Students.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Students.DAL.Enum;
using Students.MVC;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using Moq;
using AutoFixture;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Students.BLL.Services;
using Students.DAL.Tests.Classes;
using Students.MVC.Controllers;

namespace Students.BLL.Tests.RepositoryTests
{
    public class StudentRepositoryTests
    {

        private UnitOfWork UnitOfWork;
        public Fixture Fixture { get; set; } = new();

        public StudentRepositoryTests()
        {
            var myDatabaseName = "StudentsMVCBD";
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: myDatabaseName)
                .Options;
            UnitOfWork = new UnitOfWork(new Context(options));
            Fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }


        [Fact]
        public async Task SearchSurname__ActionExecutes_ReturnsViewResultNull_WithStudentsCount()
        {
            // Arrange
            const int take = 10;
            var students = Fixture.CreateMany<Student>(31);
            await UnitOfWork.StudentRepository.CreateRangeAsync(students);
            string searchString = "";

            //Act
            var result = await UnitOfWork.StudentRepository.SearchAllAsync(searchString, EnumSearchParameters.Surname, EnumPageActions.NotActions, take, 0);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchName_ActionExecutes_ReturnsViewResultSearch_WithStudentsCount()
        {
            // Arrange
            const int take = 10;
            var students = Fixture.CreateMany<Student>(31);
            await UnitOfWork.StudentRepository.CreateRangeAsync(students);
            string searchString = students.ToList()[4].Name;

            //Act
            var result = await UnitOfWork.StudentRepository.SearchAllAsync(searchString, EnumSearchParameters.Name, EnumPageActions.NotActions, take, 0);

            //Assert
            var resultSearchStudents = (await UnitOfWork.StudentRepository.GetAllAsync()).AsQueryable().Where(s => s.Name == searchString);
            Assert.Equal(resultSearchStudents.Count(), result.Count());
        }
    }
}
