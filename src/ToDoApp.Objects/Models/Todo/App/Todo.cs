using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class Todo : AModel
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public virtual Account Account { get; set; }
        public int AccountId  { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Label> Labels { get; set; }
    }
}
