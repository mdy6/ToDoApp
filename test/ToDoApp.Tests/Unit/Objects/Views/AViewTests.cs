using NSubstitute;
using System;
using Xunit;

namespace ToDoApp.Objects
{
    public class AViewTests
    {
        private AView view;

        public AViewTests()
        {
            view = Substitute.For<AView>();
        }

        [Fact]
        public void CreationDate_ReturnsSameValue()
        {
            DateTime expected = view.CreationDate;
            DateTime actual = view.CreationDate;

            Assert.Equal(expected, actual);
        }
    }
}
