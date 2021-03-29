using NSubstitute;
using System;
using Xunit;

namespace ToDoApp.Objects
{
    public class AModelTests
    {
        private AModel model;

        public AModelTests()
        {
            model = Substitute.For<AModel>();
        }

        [Fact]
        public void CreationDate_ReturnsSameValue()
        {
            DateTime expected = model.CreationDate;
            DateTime actual = model.CreationDate;

            Assert.Equal(expected, actual);
        }
    }
}
