using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class AccountRecoveryView : AView
    {
        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }
    }
}
