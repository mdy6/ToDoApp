using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ToDoApp.Resources;
using System;
using System.Collections.Generic;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class IntegerValidatorTests
    {
        [Fact]
        public void AddValidation_Integer()
        {
            Dictionary<String, String> attributes = new();
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

            new IntegerValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal(Validation.For("Integer", "Int64"), attributes["data-val-integer"]);
        }

        [Fact]
        public void AddValidation_ExistingInteger()
        {
            IModelMetadataProvider provider = new EmptyModelMetadataProvider();
            ModelMetadata metadata = provider.GetMetadataForType(typeof(Int64));
            Dictionary<String, String> attributes = new() { ["data-val-integer"] = "Test" };
            ClientModelValidationContext context = new(new ActionContext(), metadata, provider, attributes);

            new IntegerValidator().AddValidation(context);

            Assert.Single(attributes);
            Assert.Equal("Test", attributes["data-val-integer"]);
        }
    }
}
