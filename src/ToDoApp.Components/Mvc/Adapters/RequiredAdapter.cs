using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ToDoApp.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Components.Mvc
{
    public class RequiredAdapter : AttributeAdapterBase<RequiredAttribute>
    {
        public RequiredAdapter(RequiredAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For("Required");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-required"] = GetErrorMessage(context);
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
