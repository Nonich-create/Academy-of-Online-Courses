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
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

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

   
        private async Task SetUp()
        {
            Random rand = new();
            List <Course> courses = new();
            List<Manager> managers = new();
            List<Teacher> teachers = new();
            List<Group> groups = new(); 
            List<Student> students = new();
            List <CourseApplication> courseApplications = new();
            for (int i = 1; i < 6; i++)
            {
                courses.Add(Fixture.Build<Course>().With(c => c.Id, i).Create()); 
            }
            var UserManagers = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(5);
            for (int i = 1; i < UserManagers.Count(); i++)
            {
                await UnitOfWork.ApplicationUsersRepository.AddAsync(UserManagers.ToList()[i]);
                managers.Add(Fixture.Build<Manager>().With(m => m.Id,i).With(m => m.UserId,UserManagers.ToList()[i].Id).Create());
 
            }
            var UserTeachers = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(5);
            for (int i = 1; i < UserTeachers.Count(); i++)
            {
                await UnitOfWork.ApplicationUsersRepository.AddAsync(UserTeachers.ToList()[i]);
                teachers.Add(Fixture.Build<Teacher>().With(m => m.Id, i).With(m => m.UserId, UserTeachers.ToList()[i].Id).Create());

            }
           
            for (int i = 1; i < 5; i++)
            {
                groups.Add(Fixture.Build<Group>().
                With(g => g.Id,i).
                With(g => g.CourseId, courses[i].Id).
                With(g => g.TeacherId, teachers[i].Id).
                With(g => g.ManagerId, managers[i].Id).
                With(g => g.GroupStatus, GroupStatus.Set).
                With(g => g.CountMax, 30).Create());
            }

            var UserStudent = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(50);
            for (int i = 1; i < UserStudent.Count(); i++)
            {
                await UnitOfWork.ApplicationUsersRepository.AddAsync(UserStudent.ToList()[i]);
                students.Add(Fixture.Build<Student>().With(s => s.Id, i).With(s => s.UserId, UserStudent.ToList()[i].Id).Without(s => s.GroupId).Create());

            }
 
            foreach(var item in students)
            {
                courseApplications.Add(Fixture.Build<CourseApplication>().Without(c => c.Id).With(c => c.StudentId, item.Id).With(c =>c.CourseId, rand.Next(1,5)).Create());
            }

            await UnitOfWork.CourseRepository.AddRangeAsync(courses);
            await UnitOfWork.ManagerRepository.AddRangeAsync(managers);
            await UnitOfWork.TeacherRepository.AddRangeAsync(teachers);
            await UnitOfWork.GroupRepository.AddRangeAsync(groups);
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            await UnitOfWork.CourseApplicationRepository.AddRangeAsync(courseApplications);
            await UnitOfWork.SaveAsync();
        }

        [Fact]
        public async Task Enroll_courseApplicationById_Null()
        {
            //Arrange
            //Act 
            //Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(() => _courseApplicationService.Enroll(0));
        }

        [Fact]
        public async Task Enroll_studentByGroupId_Null()
        {
            //Arrange
            var group = Fixture.Build<Group>().With(g => g.Id, 1).Create();
            var student = Fixture.Build<Student>().With(s => s.Id, 1).With(s => s.GroupId, group.Id).Create();
            var courseApplication = Fixture.Build<CourseApplication>().With(c => c.Id, 1).With(c => c.StudentId, student.Id).Create();
            await UnitOfWork.GroupRepository.AddAsync(group);
            await UnitOfWork.StudentRepository.AddAsync(student);
            await UnitOfWork.CourseApplicationRepository.AddAsync(courseApplication);
            await UnitOfWork.SaveAsync();
            //Act   
            //Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(() => _courseApplicationService.Enroll(courseApplication.Id));
        }

        [Fact]
        public async Task Enroll_Group_Null()
        {
            //Arrange
            var group = Fixture.Build<Group>().With(g => g.Id, 1).Create();
            var student = Fixture.Build<Student>().With(s => s.Id, 1).Without(s => s.GroupId).Create();
            var courseApplication = Fixture.Build<CourseApplication>().With(c => c.Id, 1)
                .With(c => c.StudentId, student.Id).Create();
            var groups = Fixture.Build<Group>().With(g => g.CourseId, courseApplication.CourseId)
                .With(g => g.CountMax, 30)
                .With(g => g.GroupStatus, GroupStatus.Training).CreateMany(3);
            await UnitOfWork.GroupRepository.AddAsync(group);
            await UnitOfWork.StudentRepository.AddAsync(student);
            await UnitOfWork.CourseApplicationRepository.AddAsync(courseApplication);
            await UnitOfWork.GroupRepository.AddRangeAsync(groups);
            await UnitOfWork.SaveAsync();
            //Act   
            //Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(() => _courseApplicationService.Enroll(courseApplication.Id));
        }

        [Fact]
        public async Task Enroll_GroupByCountMax_Null()
        {
            //Arrange
            var group = Fixture.Build<Group>().With(g => g.Id, 1).Create();
            var student = Fixture.Build<Student>().With(s => s.Id, 1).Without(s => s.GroupId).Create();
            var courseApplication = Fixture.Build<CourseApplication>().With(c => c.Id, 1)
                .With(c => c.StudentId, student.Id).Create();
            var groups = Fixture.Build<Group>().With(g => g.CourseId, courseApplication.CourseId)
                .With(g => g.CountMax, 0)
                .With(g => g.GroupStatus, GroupStatus.Set).CreateMany(3);
            await UnitOfWork.GroupRepository.AddAsync(group);
            await UnitOfWork.StudentRepository.AddAsync(student);
            await UnitOfWork.CourseApplicationRepository.AddAsync(courseApplication);
            await UnitOfWork.GroupRepository.AddRangeAsync(groups);
            await UnitOfWork.SaveAsync();
            //Act   
            //Assert 
            await Assert.ThrowsAsync<InvalidOperationException>(() => _courseApplicationService.Enroll(courseApplication.Id));
        }

        [Fact]
        public async Task Enroll_Success_CourseStatusClose()
        {
            //Arrange
            var group = Fixture.Build<Group>().With(g => g.Id, 1).Create();
            var student = Fixture.Build<Student>().Without(s => s.GroupId)
                .Without(s => s.Group).With(s => s.Id, 1).Create();
            var courseApplication = Fixture.Build<CourseApplication>().With(c => c.Id, 1)
                .With(c => c.StudentId, student.Id).Create();
            var groups = Fixture.Build<Group>().With(g => g.CourseId, courseApplication.CourseId)
                .With(g => g.CountMax, 15)
                .With(g => g.GroupStatus, GroupStatus.Set).CreateMany(3);
            await UnitOfWork.GroupRepository.AddAsync(group);
            await UnitOfWork.StudentRepository.AddAsync(student);
            await UnitOfWork.CourseApplicationRepository.AddAsync(courseApplication);
            await UnitOfWork.GroupRepository.AddRangeAsync(groups);
            await UnitOfWork.SaveAsync();
            await _courseApplicationService.Enroll(courseApplication.Id);

            //Act   
            var spec = new CourseApplicationWithItemsSpecifications(student.Id);
            var result = await UnitOfWork.CourseApplicationRepository.GetAsync(spec, false);

            //Assert 
            Assert.Equal(ApplicationStatus.Close,result.ApplicationStatus);
        }

    }
}


    