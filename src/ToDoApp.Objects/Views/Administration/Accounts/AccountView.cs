using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class AccountView : AView<Account>
    {
        [StringLength(32)]
        public String Username { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }

        public Boolean IsLocked { get; set; }

        public String? RoleTitle { get; set; }
    }
}
