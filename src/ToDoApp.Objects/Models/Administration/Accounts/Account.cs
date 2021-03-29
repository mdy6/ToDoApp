using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class Account : AModel
    {
        [StringLength(32)]
        public String Username { get; set; }

        [StringLength(64)]
        public String Passhash { get; set; }

        [EmailAddress]
        [StringLength(256)]
        public String Email { get; set; }

        public Boolean IsLocked { get; set; }

        [StringLength(36)]
        public String? RecoveryToken { get; set; }
        public DateTime? RecoveryTokenExpiration { get; set; }

        public Int64? RoleId { get; set; }
        public virtual Role? Role { get; set; }

        public virtual ICollection<Todo> Todos { get; set; }
        public virtual ICollection<Label> Labels { get; set; }
    }
}
