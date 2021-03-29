using ToDoApp.Components.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class ProfileDeleteView : AView
    {
        [NotTrimmed]
        [StringLength(32)]
        public String Password { get; set; }
    }
}
