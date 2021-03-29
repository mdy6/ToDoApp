using Microsoft.AspNetCore.Mvc.ModelBinding;
using ToDoApp.Components.Notifications;
using System;

namespace ToDoApp.Validators
{
    public interface IValidator : IDisposable
    {
        Alerts Alerts { get; set; }
        ModelStateDictionary ModelState { get; set; }
    }
}
