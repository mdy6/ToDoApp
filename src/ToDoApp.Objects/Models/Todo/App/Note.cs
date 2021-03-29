using System;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class Note : AModel
    {
        public bool IsCompleted { get; set; }
        public string Text { get; set; }
        public int TodoId { get; set; }
        public virtual Todo Todo { get; set; }
    }
}
