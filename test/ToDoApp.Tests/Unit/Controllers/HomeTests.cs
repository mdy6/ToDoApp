using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using ToDoApp.Components.Extensions;
using ToDoApp.Components.Security;
using ToDoApp.Services;
using NSubstitute;
using System;
using Xunit;

namespace ToDoApp.Controllers
{
    public class HomeTests : ControllerTests
    {
        private Home controller;
        private IAccountService service;

        public HomeTests()
        {
            service = Substitute.For<IAccountService>();
            controller = Substitute.ForPartsOf<Home>(service);

            ActionContext context = new(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor());
            controller.Authorization.Returns(Substitute.For<IAuthorization>());
            controller.ControllerContext = new ControllerContext(context);
        }
        public override void Dispose()
        {
            controller.Dispose();
            service.Dispose();
        }

        [Fact]
        public void Index_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.User.Id()).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Index();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Index_ReturnsEmptyView()
        {
            service.IsActive(controller.User.Id()).Returns(true);

            ViewResult actual = Assert.IsType<ViewResult>(controller.Index());

            Assert.Null(actual.Model);
        }

        [Fact]
        public void Error_ReturnsJsonAlerts()
        {
            controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            JsonResult actual = Assert.IsType<JsonResult>(controller.Error());

            Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
            Assert.Same(controller.Alerts, actual.Value.GetType().GetProperty("alerts")?.GetValue(actual.Value));
        }

        [Fact]
        public void Error_ReturnsEmptyView()
        {
            ViewResult actual = Assert.IsType<ViewResult>(controller.Error());

            Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
            Assert.Null(actual.Model);
        }

        [Fact]
        public void NotFound_NotActive_RedirectsToLogout()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            service.IsActive(controller.User.Id()).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.NotFound();

            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void NotFound_ReturnsEmptyView(Boolean isLoggedIn, Boolean isActive)
        {
            service.IsActive(controller.User.Id()).Returns(isActive);
            service.IsLoggedIn(controller.User).Returns(isLoggedIn);

            ViewResult actual = Assert.IsType<ViewResult>(controller.NotFound());

            Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
            Assert.Null(actual.Model);
        }
    }
}
