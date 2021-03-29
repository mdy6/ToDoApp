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
    public class AppScriptTagHelperTests
    {
        private TagHelperOutput output;
        private TagHelperContext context;
        private IWebHostEnvironment host;
        private AppScriptTagHelper helper;

        public AppScriptTagHelperTests()
        {
            host = Substitute.For<IWebHostEnvironment>();
            TagHelperContent content = new DefaultTagHelperContent();
            ViewContext viewContext = MvcHelperFactory.CreateViewContext();
            IUrlHelperFactory factory = MvcHelperFactory.CreateUrlHelperFactory(viewContext);
            context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
            output = new TagHelperOutput("script", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));

            host.WebRootPath.Returns(Directory.GetCurrentDirectory());
            helper = new AppScriptTagHelper(host, factory);
            helper.ViewContext = viewContext;
            helper.Action = "Test";

            if (Directory.Exists("js"))
                Directory.Delete("js", true);
        }

        [Fact]
        public void Order_LowerThanMinusThousand()
        {
            Assert.True(helper.Order < -1000);
        }

        [Fact]
        public void Process_NoScriptRemovesTheTag()
        {
            helper.Process(context, output);

            Assert.Null(output.TagName);
        }

        [Fact]
        public void Process_CachesScriptPath()
        {
            Directory.CreateDirectory("js/application/admins/users");
            File.WriteAllText("js/application/admins/users/test.js", "script");

            helper.ViewContext!.RouteData.Values["controller"] = "Users";
            helper.ViewContext.RouteData.Values["area"] = "Admins";
            host.EnvironmentName = Environments.Development;

            helper.Process(context, output);
            File.Delete("js/application/admins/users/test.js");
            helper.Process(context, output);

            String? expected = "~/js/application/admins/users/test.js";
            String? actual = output.Attributes["src"].Value as String;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("Development", "test.js")]
        [InlineData("Production", "test.min.js")]
        public void Process_Script(String environment, String file)
        {
            Directory.CreateDirectory("js/application/admins/users");
            File.WriteAllText($"js/application/admins/users/{file}", "script");

            helper.ViewContext!.RouteData.Values["controller"] = "Users";
            helper.ViewContext.RouteData.Values["area"] = "Admins";
            host.EnvironmentName = environment;

            helper.Process(context, output);

            String? expected = $"~/js/application/admins/users/{file}";
            String? actual = output.Attributes["src"].Value as String;

            Assert.Equal(expected, actual);
        }
    }
}
