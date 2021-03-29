using ToDoApp.Objects;
using Xunit;

namespace ToDoApp.Data.Mapping
{
    public class MappingProfileTests
    {
        [Fact]
        public void Map_Role_RoleView()
        {
            Role expected = ObjectsFactory.CreateRole(0);
            RoleView actual = TestingContext.Mapper.Map<RoleView>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Empty(actual.Permissions.SelectedIds);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Empty(actual.Permissions.Nodes);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Map_RoleView_Role()
        {
            RoleView expected = ObjectsFactory.CreateRoleView(0);
            Role actual = TestingContext.Mapper.Map<Role>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Empty(actual.Permissions);
        }
    }
}
