using ToDoApp.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class AccountResetView : AView
    {
        [StringLength(36)]
        public String Token { get; set; }

        [NotTrimmed]
        [StringLength(32)]
        public String NewPassword { get; set; }
    }
}
