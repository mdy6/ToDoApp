using System;
using Xunit;

namespace ToDoApp.Components.Security
{
    public class AuthorizeAsAttributeTests
    {
        [Fact]
        public void AuthorizeAsAttribute_SetsAction()
        {
            String actual = new AuthorizeAsAttribute("Action").Action;
            String expected = "Action";

            Assert.Equal(expected, actual);
        }
    }
}
