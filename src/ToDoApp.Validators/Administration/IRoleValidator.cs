using ToDoApp.Objects;
using System;

namespace ToDoApp.Validators
{
    public interface IRoleValidator : IValidator
    {
        Boolean CanCreate(RoleView view);
        Boolean CanEdit(RoleView view);
    }
}
