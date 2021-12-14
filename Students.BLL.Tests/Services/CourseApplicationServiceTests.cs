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

   
        private async Task Setup()
        {
            Random rand = new();
            List <Course> courses = new();
            List<Manager> managers = new();
            List<Teacher> teachers = new();
            List<Student> students = new();
            List <CourseApplication> courseApplications = new();
            for (int i = 1; i < 6; i++)
            {
                courses.Add(Fixture.Build<Course>().Without(c => c.Id).Create());
            }
            await UnitOfWork.CourseRepository.AddRangeAsync(courses);

            var UserManagers = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(5);
            await UnitOfWork.ApplicationUsersRepository.AddRangeAsync(UserManagers);
            foreach (var item in UserManagers)
            {
                managers.Add(Fixture.Build<Manager>().Without(m => m.Id)
                    .With(m => m.UserId, item.Id).Create());

            }
            await  UnitOfWork.ManagerRepository.AddRangeAsync(managers);

            var UserTeachers = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(5);
            await UnitOfWork.ApplicationUsersRepository.AddRangeAsync(UserTeachers);
            foreach (var item in UserTeachers)
            {
                teachers.Add(Fixture.Build<Teacher>().Without(m => m.Id)
                    .With(m => m.UserId, item.Id).Create());
            }
            await UnitOfWork.TeacherRepository.AddRangeAsync(teachers);

                var groups =(Fixture.Build<Group>().
                Without(g => g.Id).
                With(g => g.CourseId, rand.Next(1, 5)).
                With(g => g.TeacherId, rand.Next(1, 5)).
                With(g => g.ManagerId, rand.Next(1, 5)).
                With(g => g.GroupStatus, GroupStatus.Set).
                With(g => g.CountMax, 30).CreateMany<Group>(6));

            await UnitOfWork.GroupRepository.AddRangeAsync(groups);


            var UserStudents = Fixture.Build<ApplicationUser>().CreateMany<ApplicationUser>(50);
            await UnitOfWork.ApplicationUsersRepository.AddRangeAsync(UserStudents);
            foreach (var item in UserStudents)
            {
                students.Add(Fixture.Build<Student>().Without(s => s.Id).With(s => s.UserId, item.Id)
                    .Without(s => s.GroupId).Create());
            }
            await UnitOfWork.StudentRepository.AddRangeAsync(students);
            foreach (var item in students)
            {
                courseApplications.Add(Fixture.Build<CourseApplication>().Without(c => c.Id)
                    .With(c => c.StudentId, item.Id).With(c =>c.CourseId, rand.Next(1,5)).Create());
            }
            await  UnitOfWork.CourseApplicationRepository.AddRangeAsync(courseApplications);
           
        }

        [Fact]
        public async Task E1nroll_courseApplicationById_Null()
        {
            await Setup();
            await UnitOfWork.SaveAsync();
            //Arrange
            var student = await UnitOfWork.StudentRepository.GetAllAsync();
            var course = await UnitOfWork.CourseRepository.GetAllAsync();
            ////Act 
            ////Assert 
            //await Assert.ThrowsAsync<InvalidOperationException>(() => _courseApplicationService.Enroll(0));
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


    