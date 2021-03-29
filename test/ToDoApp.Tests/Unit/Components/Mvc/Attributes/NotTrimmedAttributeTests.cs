using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class NotTrimmedAttributeTests
    {
        [Fact]
        public void NotTrimmedAttribute_SetsBinderType()
        {
            Type actual = new NotTrimmedAttribute().BinderType;
            Type expected = typeof(NotTrimmedAttribute);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_DoesNotTrimValue()
        {
            DefaultModelBindingContext context = new()
            {
                ModelName = "Test",
                ModelState = new ModelStateDictionary(),
                ActionContext = MvcHelperFactory.CreateViewContext(),
                ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(String)),
                ValueProvider = new RouteValueProvider(BindingSource.Path, new RouteValueDictionary(new { Test = " Value  " }))
            };

            await new NotTrimmedAttribute().BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(" Value  ");
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected, actual);
        }
    }
}
