using Microsoft.Extensions.Logging;
using Xunit;

namespace ToDoApp.Components.Logging
{
    public class FileLoggerProviderTests
    {
        [Fact]
        public void Create_SameInstance()
        {
            using FileLoggerProvider provider = new("log.txt", 10);

            ILogger expected = provider.CreateLogger("1");
            ILogger actual = provider.CreateLogger("2");

            Assert.Same(expected, actual);
        }
    }
}
