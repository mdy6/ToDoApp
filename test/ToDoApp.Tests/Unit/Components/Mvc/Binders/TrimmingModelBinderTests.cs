using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class TrimmingModelBinderTests
    {
        private TrimmingModelBinder binder;
        private ModelBindingContext context;
        private RouteValueDictionary values;

        public TrimmingModelBinderTests()
        {
            binder = new TrimmingModelBinder();
            values = new RouteValueDictionary();
            context = new DefaultModelBindingContext();
            context.ModelState = new ModelStateDictionary();
            context.ValueProvider = new RouteValueProvider(BindingSource.Path, values);
            context.ModelMetadata = Substitute.For<ModelMetadata>(ModelMetadataIdentity.ForType(typeof(String)));
        }

        [Fact]
        public async Task BindModelAsync_NoValue()
        {
            context.ModelName = "Test";

            await binder.BindModelAsync(context);

            ModelBindingResult actual = context.Result;
            ModelBindingResult expected = new();

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        public async Task BindModelAsync_Null(String value)
        {
            context.ModelMetadata.ConvertEmptyStringToNull.Returns(true);
            context.ModelName = nameof(AllTypesView.StringField);
            values[nameof(AllTypesView.StringField)] = value;
            context.ModelMetadata.IsRequired.Returns(false);

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success(null);
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        [Theory]
        [InlineData(true, true, "")]
        [InlineData(false, true, "  ")]
        [InlineData(false, false, "  ")]
        public async Task BindModelAsync_Empty(Boolean convertToNull, Boolean isRequired, String value)
        {
            context.ModelMetadata.ConvertEmptyStringToNull.Returns(convertToNull);
            context.ModelMetadata.IsRequired.Returns(isRequired);
            context.ModelName = nameof(AllTypesView.StringField);
            values[nameof(AllTypesView.StringField)] = value;

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success("");
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }

        [Fact]
        public async Task BindModelAsync_Trimmed()
        {
            values["Test"] = " Value ";
            context.ModelName = "Test";

            await binder.BindModelAsync(context);

            ModelBindingResult expected = ModelBindingResult.Success("Value");
            ModelBindingResult actual = context.Result;

            Assert.Equal(expected.IsModelSet, actual.IsModelSet);
            Assert.Equal(expected.Model, actual.Model);
        }
    }
}
