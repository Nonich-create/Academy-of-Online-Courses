using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Students.BLL.Tests.Classes
{
    public class BasicTests
        : IClassFixture<WebApplicationFactory<Students.MVC.Startup>>
    {
        private readonly WebApplicationFactory<Students.MVC.Startup> _factory;

        public BasicTests(WebApplicationFactory<Students.MVC.Startup> factory)
        {
            _factory = factory;
        }
    }
}
