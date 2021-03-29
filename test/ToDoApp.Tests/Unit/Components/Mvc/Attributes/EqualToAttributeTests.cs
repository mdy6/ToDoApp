using ToDoApp.Resources;
using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class EqualToAttributeTests : IDisposable
    {
        private EqualToAttribute attribute;

        public EqualToAttributeTests()
        {
            attribute = new EqualToAttribute(nameof(AllTypesView.StringField));
            Resource.Set(nameof(AllTypesView))["", "Titles", nameof(AllTypesView.StringField)] = "Other title";
        }
        public void Dispose()
        {
            Resource.Set(nameof(AllTypesView))["", "Titles", nameof(AllTypesView.StringField)] = null;
        }

        [Fact]
        public void EqualToAttribute_SetsOtherPropertyName()
        {
            String actual = new EqualToAttribute("OtherProperty").OtherPropertyName;
            String expected = "OtherProperty";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FormatErrorMessage_ForProperty()
        {
            attribute.OtherPropertyDisplayName = "Other";

            String actual = attribute.FormatErrorMessage("EqualTo");
            String expected = Validation.For("EqualTo", "EqualTo", attribute.OtherPropertyDisplayName);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidationResult_EqualValue()
        {
            ValidationContext context = new(new AllTypesView { StringField = "Test" });

            Assert.Null(attribute.GetValidationResult("Test", context));
        }

        [Fact]
        public void GetValidationResult_Property_Error()
        {
            ValidationContext context = new(new AllTypesView());

            String? expected = Validation.For("EqualTo", context.DisplayName, "Other title");
            String? actual = attribute.GetValidationResult("Test", context)?.ErrorMessage;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetValidationResult_NoProperty_Error()
        {
            attribute = new EqualToAttribute("Temp");
            ValidationContext context = new(new AllTypesView());

            String? expected = Validation.For("EqualTo", context.DisplayName, "Temp");
            String? actual = attribute.GetValidationResult("Test", context)?.ErrorMessage;

            Assert.Equal(expected, actual);
        }
    }
}
