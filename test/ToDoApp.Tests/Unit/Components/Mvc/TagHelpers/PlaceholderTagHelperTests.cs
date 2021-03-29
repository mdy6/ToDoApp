using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class PlaceholderTagHelperTests
    {
        [Fact]
        public void Process_Placeholder()
        {
            TagHelperContent content = new DefaultTagHelperContent();
            ModelMetadata metadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
            TagHelperContext context = new(new TagHelperAttributeList(), new Dictionary<Object, Object>(), "test");
            TagHelperOutput output = new("input", new TagHelperAttributeList(), (_, _) => Task.FromResult(content));
            PlaceholderTagHelper helper = new() { For = new ModelExpression("Total", new ModelExplorer(metadata, metadata, null)) };

            metadata.DisplayName.Returns("Test");
            helper.Process(context, output);

            Assert.Single(output.Attributes);
            Assert.Empty(output.Content.GetContent());
            Assert.Equal("Test", output.Attributes["placeholder"].Value);
        }
    }
}
