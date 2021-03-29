using Microsoft.AspNetCore.Mvc.ModelBinding;
using ToDoApp.Components.Notifications;
using ToDoApp.Data;
using ToDoApp.Objects;
using ToDoApp.Resources;
using System;
using System.Linq.Expressions;

namespace ToDoApp.Validators
{
    public abstract class AValidator : IValidator
    {
        public Alerts Alerts { get; set; }
        public ModelStateDictionary ModelState { get; set; }

        protected IUnitOfWork UnitOfWork { get; }

        protected AValidator(IUnitOfWork unitOfWork)
        {
            ModelState = new ModelStateDictionary();
            UnitOfWork = unitOfWork;
            Alerts = new Alerts();
        }

        protected Boolean IsSpecified<TView>(TView view, Expression<Func<TView, Object?>> property) where TView : AView
        {
            Boolean isSpecified = property.Compile().Invoke(view) != null;

            if (!isSpecified)
            {
                if (property.Body is UnaryExpression unary)
                    ModelState.AddModelError(property, Validation.For("Required", Resource.ForProperty(unary.Operand)));
                else
                    ModelState.AddModelError(property, Validation.For("Required", Resource.ForProperty(property)));
            }

            return isSpecified;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
