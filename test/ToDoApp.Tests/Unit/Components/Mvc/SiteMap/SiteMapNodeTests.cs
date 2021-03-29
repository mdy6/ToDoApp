using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class SiteMapNodeTests
    {
        private IUrlHelper helper;

        public SiteMapNodeTests()
        {
            helper = Substitute.For<IUrlHelper>();
            ViewContext context = MvcHelperFactory.CreateViewContext();

            helper.ActionContext.Returns(context);
            helper.Action(Arg.Any<UrlActionContext>()).Returns(info =>
            {
                UrlActionContext context = info.Arg<UrlActionContext>();
                RouteValueDictionary route = new(info.Arg<UrlActionContext>().Values);
                String query = String.Join("&", route.Where(pair => pair.Key != "area").Select(pair => $"{pair.Key}={pair.Value}"));

                return $"{route["area"]}/{context.Controller}/{context.Action}?{query}";
            });
        }

        [Theory]
        [InlineData("", "B", null, "#")]
        [InlineData("A", "B", null, "#")]
        [InlineData(null, "B", null, "#")]
        [InlineData("", "B", "C", "/B/C?")]
        [InlineData("A", "B", "C", "A/B/C?")]
        [InlineData(null, "B", "C", "/B/C?")]
        public void Form_Url(String area, String controller, String action, String url)
        {
            SiteMapNode node = new() { Area = area, Controller = controller, Action = action };

            String actual = node.Form(helper);
            String expected = url;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Form_RouteUrl()
        {
            helper.ActionContext.HttpContext.Request.Query = new QueryCollection(QueryHelpers.ParseQuery("one=lamp&value=5"));
            SiteMapNode node = new() { Area = "A", Controller = "B", Action = "C" };
            helper.ActionContext.RouteData.Values["reroute"] = "D";
            helper.ActionContext.RouteData.Values["value"] = null;
            helper.ActionContext.RouteData.Values["id"] = "1000";
            node.Route["area"] = "reroute";
            node.Route["value"] = "value";
            node.Route["test"] = "id";
            node.Route["id"] = "one";

            String expected = "D/B/C?value=5&test=1000&id=lamp";
            String actual = node.Form(helper);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToBreadcrumb_List()
        {
            SiteMapNode first = new() { Action = "A" };
            SiteMapNode firstFiller = new() { Parent = first };
            SiteMapNode second = new() { Action = "C", Parent = firstFiller };
            SiteMapNode secondFiller = new() { Parent = second };
            SiteMapNode third = new() { Action = "E", Parent = secondFiller };

            SiteMapNode[] expected = { first, second, third };
            SiteMapNode[] actual = third.ToBreadcrumb();

            Assert.Equal(expected, actual);
        }
    }
}
