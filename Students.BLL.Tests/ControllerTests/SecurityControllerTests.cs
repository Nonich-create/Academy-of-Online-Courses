using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Students.BLL.Services;
using Students.DAL.Models;
using Students.DAL.Tests.Classes;
using Students.MVC.Helpers;
using Students.MVC.Controllers;
using Students.MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace Students.DAL.Tests.ControllerTests
{
    public  class SecurityControllerTests
    {
        public SecurityController SecurityController { get; set; }
        public Mock<IUserService> UserServicMock { get; } = new Mock<IUserService>();
        public Mock<ILogger<SecurityController>> LoggerMock { get; } = new Mock<ILogger<SecurityController>>();

        public SecurityControllerTests()
        {
            var fakeUser = new FakeUserManager();
            var signInManager = new FakeSignInManager();
            SecurityController = new SecurityController(LoggerMock.Object, fakeUser, signInManager, UserServicMock.Object);
        }

        [Fact]
        public void Authorization_ActionExecutes_ReturnsViewForAuthorization()
        {
            // Act
            var result =  SecurityController.Authorization();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
