using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using ToDoApp.Resources;
using System;

namespace ToDoApp.Components.Mvc
{
    public class DisplayMetadataProvider : IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.ContainerType is Type view && context.Key.MetadataKind == ModelMetadataKind.Property)
                if (Resource.ForProperty(view, context.Key.Name!) is String { Length: > 0 } title)
                    context.DisplayMetadata.DisplayName = () => title;
        }
    }
}
