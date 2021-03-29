using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class Label : AModel
    {
        public string Name { get; set; }
        public virtual ICollection<Todo> Todos { get; set; }
        public virtual Account Account { get; set; }
        public int AccountId { get; set; }
    }
}
