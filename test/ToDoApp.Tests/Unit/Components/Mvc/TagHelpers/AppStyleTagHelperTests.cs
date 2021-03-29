using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class AppStyleTagHelperTests
    {
        private TagHelperOutput output;
        private TagHelperContext context;
        private AppStyleTagHelper helper;
        private IWebHostEnvironment host;

        public AppStyleTagHelperTests()
        {
            host = Substitute.For<IWebHostEnvironment>();
            TagHelperContent content = new DefaultTagHelperContent();
            ViewContext viewContext = MvcHelperFactory.CreateViewContext();
            IUrlHelperFactory factory = MvcHelperFactory.CreateUrlHelperFactory(viewContext);
            context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
            output = new TagHelperOutput("link", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));

            host.WebRootPath.Returns(Directory.GetCurrentDirectory());
            helper = new AppStyleTagHelper(host, factory);
            helper.ViewContext = viewContext;
            helper.Action = "Test";

            if (Directory.Exists("css"))
                Directory.Delete("css", true);
        }

        [Fact]
        public void Order_LowerThanMinusThousand()
        {
            Assert.True(helper.Order < -1000);
        }

        [Fact]
        public void Process_NoStyleRemovesTheTag()
        {
            helper.Process(context, output);

            Assert.Null(output.TagName);
        }

        [Fact]
        public void Process_CachesStylePath()
        {
            Directory.CreateDirectory("css/application/admins/users");
            File.WriteAllText("css/application/admins/users/test.css", "link");

            helper.ViewContext!.RouteData.Values["controller"] = "Users";
            helper.ViewContext.RouteData.Values["area"] = "Admins";
            host.EnvironmentName = Environments.Development;

            helper.Process(context, output);
            File.Delete("css/application/admins/users/test.css");
            helper.Process(context, output);

            String? expected = "~/css/application/admins/users/test.css";
            String? actual = output.Attributes["href"].Value as String;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Development", "test.css")]
        [InlineData("Production", "test.min.css")]
        public void Process_Style(String environment, String file)
        {
            Directory.CreateDirectory("css/application/admins/users");
            File.WriteAllText($"css/application/admins/users/{file}", "link");

            helper.ViewContext!.RouteData.Values["controller"] = "Users";
            helper.ViewContext.RouteData.Values["area"] = "Admins";
            host.EnvironmentName = environment;

            helper.Process(context, output);

            String? expected = $"~/css/application/admins/users/{file}";
            String? actual = output.Attributes["href"].Value as String;

            Assert.Equal(expected, actual);
        }
    }
}
