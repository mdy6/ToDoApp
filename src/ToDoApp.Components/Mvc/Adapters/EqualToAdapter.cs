using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ToDoApp.Resources;
using System;

namespace ToDoApp.Components.Mvc
{
    public class EqualToAdapter : AttributeAdapterBase<EqualToAttribute>
    {
        public EqualToAdapter(EqualToAttribute attribute)
            : base(attribute, null)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            Attribute.OtherPropertyDisplayName = Resource.ForProperty(context.ModelMetadata.ContainerType!, Attribute.OtherPropertyName);

            if (String.IsNullOrEmpty(Attribute.OtherPropertyDisplayName))
                Attribute.OtherPropertyDisplayName = Attribute.OtherPropertyName;

            context.Attributes["data-val-equalto"] = GetErrorMessage(context);
            context.Attributes["data-val-equalto-other"] = Attribute.OtherPropertyName;
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
