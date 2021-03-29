using System;
using System.Collections.Concurrent;

namespace ToDoApp.Resources
{
    internal class ResourceDictionary : ConcurrentDictionary<String, String?>
    {
        public ResourceDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
