using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using ToDoApp.Components.Extensions;
using ToDoApp.Components.Notifications;
using ToDoApp.Components.Security;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace ToDoApp.Controllers
{
    public class AControllerTests : ControllerTests
    {
        private ActionExecutingContext context;
        private AController controller;
        private String controllerName;
        private ActionContext action;
        private String? areaName;

        public AControllerTests()
        {
            controller = Substitute.ForPartsOf<AController>();

            controller.Url = Substitute.For<IUrlHelper>();
            controller.ControllerContext.RouteData = new RouteData();
            controller.TempData = Substitute.For<ITempDataDictionary>();
            controller.Authorization.Returns(Substitute.For<IAuthorization>());
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());

            areaName = controller.RouteData.Values["area"] as String;
            controllerName = (String)controller.RouteData.Values["controller"]!;
            action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            context = new ActionExecutingContext(action, new List<IFilterMetadata>(), new Dictionary<String, Object>(), controller);
        }
        public override void Dispose()
        {
            controller.Dispose();
        }

        [Fact]
        public void AController_CreatesEmptyAlerts()
        {
            Assert.Empty(controller.Alerts);
        }

        [Fact]
        public void NotFoundView_ReturnsNotFoundView()
        {
            ViewResult actual = controller.NotFoundView();

            Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
            Assert.Equal($"~/Views/{nameof(Home)}/{nameof(Home.NotFound)}.cshtml", actual.ViewName);
        }

        [Fact]
        public void NotEmptyView_NullModel_ReturnsNotFoundView()
        {
            ViewResult expected = NotFoundView(controller);
            ViewResult actual = controller.NotEmptyView(null);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void NotEmptyView_ReturnsModelView()
        {
            Object expected = new();
            Object actual = Assert.IsType<ViewResult>(controller.NotEmptyView(expected)).Model;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RedirectToLocal_NotLocalUrl_RedirectsToDefault()
        {
            controller.Url.IsLocalUrl("T").Returns(false);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.RedirectToLocal("T");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RedirectToLocal_IsLocalUrl_RedirectsToLocal()
        {
            controller.Url.IsLocalUrl("/").Returns(true);

            String actual = Assert.IsType<RedirectResult>(controller.RedirectToLocal("/")).Url;
            String expected = "/";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RedirectToDefault_Route()
        {
            RedirectToActionResult actual = controller.RedirectToDefault();

            Assert.Equal(nameof(Home.Index), actual.ActionName);
            Assert.Equal(nameof(Home), actual.ControllerName);
            Assert.Equal("", actual.RouteValues["area"]);
            Assert.Single(actual.RouteValues);
        }

        [Fact]
        public void IsAuthorizedFor_ReturnsAuthorizationResult()
        {
            controller.Authorization.IsGrantedFor(controller.User.Id(), "Area/Controller/Action").Returns(true);

            Assert.True(controller.IsAuthorizedFor("Area/Controller/Action"));
        }

        [Fact]
        public void RedirectToAction_Action_Controller_Route_NotAuthorized_RedirectsToDefault()
        {
            controller.IsAuthorizedFor($"{areaName}/Controller/Action").Returns(false);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.RedirectToAction("Action", "Controller", new { id = 1 });

            Assert.Same(expected, actual);
        }

        [Fact]
        public void RedirectToAction_Action_NullController_NullRoute_RedirectsToAction()
        {
            controller.IsAuthorizedFor($"{areaName}/{controllerName}/Action").Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", null, null);

            Assert.Equal(controllerName, actual.ControllerName);
            Assert.Equal("Action", actual.ActionName);
            Assert.Null(actual.RouteValues);
        }

        [Fact]
        public void RedirectToAction_Action_Controller_NullRoute_RedirectsToAction()
        {
            controller.IsAuthorizedFor($"{areaName}/Controller/Action").Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", "Controller", null);

            Assert.Equal("Controller", actual.ControllerName);
            Assert.Equal("Action", actual.ActionName);
            Assert.Null(actual.RouteValues);
        }

        [Fact]
        public void RedirectToAction_Action_Controller_Route_RedirectsToAction()
        {
            controller.IsAuthorizedFor("Area/Controller/Action").Returns(true);

            RedirectToActionResult actual = controller.RedirectToAction("Action", "Controller", new { area = "Area", id = 1 });

            Assert.Equal("Controller", actual.ControllerName);
            Assert.Equal("Area", actual.RouteValues["area"]);
            Assert.Equal("Action", actual.ActionName);
            Assert.Equal(1, actual.RouteValues["id"]);
            Assert.Equal(2, actual.RouteValues.Count);
        }

        [Fact]
        public void OnActionExecuting_SetsAuthorization()
        {
            controller = Substitute.ForPartsOf<AController>();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());

            controller.OnActionExecuting(context);

            Object expected = controller.HttpContext.RequestServices.GetRequiredService<IAuthorization>();
            Object actual = controller.Authorization;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuted_JsonResult_NoAlerts()
        {
            JsonResult result = new("Value");
            controller.Alerts.AddError("Test");
            controller.TempData["Alerts"] = null;

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), controller) { Result = result });

            Assert.Null(controller.TempData["Alerts"]);
        }

        [Fact]
        public void OnActionExecuted_NullTempDataAlerts_SetsTempDataAlerts()
        {
            controller.Alerts.AddError("Test");
            controller.TempData["Alerts"] = null;

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), controller));

            Object expected = JsonSerializer.Serialize(controller.Alerts);
            Object actual = controller.TempData["Alerts"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuted_MergesTempDataAlerts()
        {
            Alerts alerts = new();
            alerts.AddError("Test1");

            controller.TempData["Alerts"] = JsonSerializer.Serialize(alerts);

            controller.Alerts.AddError("Test2");
            alerts.AddError("Test2");

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), controller));

            Object expected = JsonSerializer.Serialize(alerts);
            Object actual = controller.TempData["Alerts"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuted_NoAlerts()
        {
            controller.Alerts.Clear();

            controller.OnActionExecuted(new ActionExecutedContext(action, new List<IFilterMetadata>(), controller));

            Assert.Null(controller.TempData["Alerts"]);
        }
    }
}
