using ToDoApp.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Components.Mvc
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class LessThanAttribute : ValidationAttribute
    {
        public Decimal Maximum { get; }

        public LessThanAttribute(Double maximum)
            : base(() => Validation.For("LessThan"))
        {
            Maximum = Convert.ToDecimal(maximum);
        }

        public override String FormatErrorMessage(String name)
        {
            return String.Format(ErrorMessageString, name, Maximum);
        }
        public override Boolean IsValid(Object? value)
        {
            try
            {
                return value == null || Convert.ToDecimal(value) < Maximum;
            }
            catch
            {
                return false;
            }
        }
    }
}
