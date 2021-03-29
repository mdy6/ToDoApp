using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ToDoApp.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Components.Mvc
{
    public class EmailAddressAdapter : AttributeAdapterBase<EmailAddressAttribute>
    {
        public EmailAddressAdapter(EmailAddressAttribute attribute)
            : base(attribute, null)
        {
            attribute.ErrorMessage = Validation.For("EmailAddress");
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val-email"] = GetErrorMessage(context);
        }
        public override String GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return GetErrorMessage(validationContext.ModelMetadata);
        }
    }
}
