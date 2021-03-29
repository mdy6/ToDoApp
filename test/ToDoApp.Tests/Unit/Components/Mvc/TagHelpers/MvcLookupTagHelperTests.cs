using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class MvcLookupTagHelperTests
    {
        [Theory]
        [InlineData("Test", "Test")]
        [InlineData(null, "~/Lookup/Handling")]
        public void Process_Url(String? url, String newUrl)
        {
            TagHelperContent content = new DefaultTagHelperContent();
            IUrlHelperFactory factory = MvcHelperFactory.CreateUrlHelperFactory(null);
            MvcLookupTagHelper helper = new(Substitute.For<IHtmlGenerator>(), factory);
            TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
            TagHelperOutput output = new("div", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));

            helper.Url = url;
            helper.Handler = "Handling";

            helper.Process(context, output);

            String? expected = newUrl;
            String? actual = helper.Url;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(null, "Roles")]
        [InlineData("Testing", "Testing")]
        public void Process_Title(String? title, String newTitle)
        {
            TagHelperContent content = new DefaultTagHelperContent();
            TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
            TagHelperOutput output = new("div", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
            MvcLookupTagHelper helper = new(Substitute.For<IHtmlGenerator>(), Substitute.For<IUrlHelperFactory>());

            helper.Title = title;
            helper.Handler = "Role";

            helper.Process(context, output);

            String? expected = newTitle;
            String? actual = helper.Title;

            Assert.Equal(expected, actual);
        }
    }
}
