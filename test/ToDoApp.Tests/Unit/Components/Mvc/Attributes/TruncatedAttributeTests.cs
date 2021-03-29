using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class TruncatedAttributeTests
    {
        private TruncatedAttribute attribute;
        private RouteValueDictionary values;
        private ModelBindingContext context;

        public TruncatedAttributeTests()
        {
            values = new RouteValueDictionary();
            attribute = new TruncatedAttribute();
            context = new DefaultModelBindingContext();
            context.ModelState = new ModelStateDictionary();
            context.ModelName = "TruncatedDateTimeParameter";
            context.ActionContext = MvcHelperFactory.CreateViewContext();
            context.ValueProvider = new RouteValueProvider(BindingSource.Path, values);
            context.ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(DateTime?));
        }

        [Fact]
        public void TruncatedAttribute_SetsBinderType()
        {
            Type expected = typeof(TruncatedAttribute);
            Type actual = attribute.BinderType;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_NoValue()
        {
            await attribute.BindModelAsync(context);

            ModelBindingResult actual = context.Result;
            ModelBindingResult expected = new();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task BindModelAsync_TruncatesValue()
        {
            values[context.ModelName] = new DateTime(2017, 2, 3, 4, 5, 6).ToString();

            await attribute.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(new DateTime(2017, 2, 3));
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }
    }
}
