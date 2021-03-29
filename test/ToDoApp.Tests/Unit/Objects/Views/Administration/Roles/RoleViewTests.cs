using ToDoApp.Components.Tree;
using Xunit;

namespace ToDoApp.Objects
{
    public class RoleViewTests
    {
        [Fact]
        public void RoleView_CreatesEmpty()
        {
            MvcTree actual = new RoleView().Permissions;

            Assert.Empty(actual.SelectedIds);
            Assert.Empty(actual.Nodes);
        }
    }
}
