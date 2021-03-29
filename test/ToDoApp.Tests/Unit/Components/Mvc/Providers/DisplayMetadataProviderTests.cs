using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using ToDoApp.Objects;
using ToDoApp.Resources;
using System;
using System.Reflection;
using Xunit;

namespace ToDoApp.Components.Mvc
{
    public class DisplayMetadataProviderTests
    {
        [Fact]
        public void CreateDisplayMetadata_NullContainerType_DoesNotSetDisplayName()
        {
            DisplayMetadataProvider provider = new();
            DisplayMetadataProviderContext context = new(
                   ModelMetadataIdentity.ForType(typeof(RoleView)),
                   ModelAttributes.GetAttributesForType(typeof(RoleView)));

            provider.CreateDisplayMetadata(context);

            Assert.Null(context.DisplayMetadata.DisplayName);
        }

        [Fact]
        public void CreateDisplayMetadata_NullResource_DoesNotSetDisplayName()
        {
            PropertyInfo property = typeof(AllTypesView).GetProperty(nameof(AllTypesView.StringField))!;
            DisplayMetadataProviderContext context = new(
                ModelMetadataIdentity.ForProperty(property, typeof(String), typeof(AllTypesView)),
                ModelAttributes.GetAttributesForType(typeof(AllTypesView)));

            new DisplayMetadataProvider().CreateDisplayMetadata(context);

            Assert.Null(context.DisplayMetadata.DisplayName);
        }

        [Fact]
        public void CreateDisplayMetadata_SetsDisplayName()
        {
            PropertyInfo property = typeof(RoleView).GetProperty(nameof(RoleView.Title))!;
            DisplayMetadataProviderContext context = new(
                ModelMetadataIdentity.ForProperty(property, typeof(String), typeof(RoleView)),
                ModelAttributes.GetAttributesForType(typeof(RoleView)));

            new DisplayMetadataProvider().CreateDisplayMetadata(context);

            String expected = Resource.ForProperty(typeof(RoleView), nameof(RoleView.Title));
            String actual = context.DisplayMetadata.DisplayName();

            Assert.Equal(expected, actual);
        }
    }
}
