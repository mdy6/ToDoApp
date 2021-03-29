using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToDoApp.Components.Security;
using NSubstitute;
using System;
using System.Security.Claims;

namespace ToDoApp
{
    public static class MvcHelperFactory
    {
        public static IHtmlHelper CreateHtmlHelper()
        {
            IHtmlHelper<Object> html = Substitute.For<IHtmlHelper<Object>>();
            ViewContext context = CreateViewContext();
            html.ViewContext.Returns(context);

            return html;
        }
        public static ViewContext CreateViewContext()
        {
            ViewContext context = new();
            IUrlHelperFactory factory = CreateUrlHelperFactory(context);
            ClaimsIdentity identity = new(new[] { new Claim(ClaimTypes.NameIdentifier, "1") });

            context.RouteData = new RouteData();
            context.HttpContext = new DefaultHttpContext();
            context.HttpContext.Request.Path = "/en/home/index";
            context.HttpContext.User = new ClaimsPrincipal(identity);
            context.HttpContext.RequestServices = Substitute.For<IServiceProvider>();
            context.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);
            context.HttpContext.RequestServices.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());
            context.HttpContext.RequestServices.GetService(typeof(ILoggerFactory)).Returns(Substitute.For<ILoggerFactory>());
            context.HttpContext.RequestServices.GetService(typeof(IServiceScopeFactory)).Returns(Substitute.For<IServiceScopeFactory>());

            return context;
        }
        public static IUrlHelperFactory CreateUrlHelperFactory(ActionContext? context)
        {
            IUrlHelper url = Substitute.For<IUrlHelper>();
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();

            url.ActionContext.Returns(context);
            factory.GetUrlHelper(context).Returns(url);
            url.Content(Arg.Any<String>()).Returns(info => info.Arg<String>());

            return factory;
        }
    }
}
