using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Components.Tree;
using ToDoApp.Controllers;
using ToDoApp.Controllers.Administration;
using ToDoApp.Data;
using ToDoApp.Objects;
using ToDoApp.Resources;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ToDoApp.Services
{
    public class RoleServiceTests : IDisposable
    {
        private RoleService service;
        private DbContext context;
        private Role role;

        public RoleServiceTests()
        {
            context = TestingContext.Create();
            service = Substitute.ForPartsOf<RoleService>(new UnitOfWork(TestingContext.Create(), TestingContext.Mapper));

            role = SetUpData();
        }
        public void Dispose()
        {
            service.Dispose();
            context.Dispose();
        }

        [Fact]
        public void GetViews_ReturnsRoleViews()
        {
            RoleView[] actual = service.GetViews().ToArray();
            RoleView[] expected = context
                .Set<Role>()
                .ProjectTo<RoleView>(TestingContext.Mapper.ConfigurationProvider)
                .OrderByDescending(view => view.Id)
                .ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].Permissions.SelectedIds, actual[i].Permissions.SelectedIds);
                Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        [Fact]
        public void GetView_NoRole_ReturnsNull()
        {
            Assert.Null(service.GetView(0));
        }

        [Fact]
        public void GetView_ReturnsViewById()
        {
            RoleView expected = TestingContext.Mapper.Map<RoleView>(role);
            RoleView actual = service.GetView(role.Id)!;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.NotEmpty(actual.Permissions.SelectedIds);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void GetView_SetsSelectedIds()
        {
            IEnumerable<Int64> expected = role.Permissions.Select(rolePermission => rolePermission.PermissionId).OrderBy(id => id);
            IEnumerable<Int64> actual = service.GetView(role.Id)!.Permissions.SelectedIds.OrderBy(id => id);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetView_SeedsPermissions()
        {
            RoleView view = service.GetView(role.Id)!;

            service.Received().Seed(view.Permissions);
        }

        [Fact]
        public void Seed_FirstDepth()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Seed(view.Permissions);

            List<MvcTreeNode> expected = CreatePermissions().Nodes;
            List<MvcTreeNode> actual = view.Permissions.Nodes;

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void Seed_SecondDepth()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Seed(view.Permissions);

            List<MvcTreeNode> expected = CreatePermissions().Nodes.SelectMany(node => node.Children).ToList();
            List<MvcTreeNode> actual = view.Permissions.Nodes.SelectMany(node => node.Children).ToList();

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void Seed_ThirdDepth()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Seed(view.Permissions);

            List<MvcTreeNode> expected = CreatePermissions().Nodes.SelectMany(node => node.Children).SelectMany(node => node.Children).OrderBy(node => node.Id).ToList();
            List<MvcTreeNode> actual = view.Permissions.Nodes.SelectMany(node => node.Children).SelectMany(node => node.Children).OrderBy(node => node.Id).ToList();

            for (Int32 i = 0; i < expected.Count || i < actual.Count; i++)
            {
                Assert.Equal(expected[i].Id, actual[i].Id);
                Assert.Equal(expected[i].Title, actual[i].Title);
                Assert.Equal(expected[i].Children.Count, actual[i].Children.Count);
            }
        }

        [Fact]
        public void Seed_BranchesWithoutId()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Seed(view.Permissions);

            IEnumerable<MvcTreeNode> nodes = view.Permissions.Nodes;
            IEnumerable<MvcTreeNode> branches = GetBranchNodes(nodes);

            Assert.Empty(branches.Where(branch => branch.Id != null));
        }

        [Fact]
        public void Seed_LeafsWithId()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Seed(view.Permissions);

            IEnumerable<MvcTreeNode> actual = GetLeafNodes(view.Permissions.Nodes);

            Assert.Empty(actual.Where(leaf => leaf.Id == null));
            Assert.Equal(4, actual.Count());
        }

        [Fact]
        public void Create_Role()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);

            service.Create(view);

            Role actual = Assert.Single(context.Set<Role>(), model => model.Id != role.Id);
            RoleView expected = view;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
        }

        [Fact]
        public void Create_RolePermissions()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id + 1);
            view.Permissions = CreatePermissions();

            service.Create(view);

            IEnumerable<Int64> expected = view.Permissions.SelectedIds.OrderBy(permissionId => permissionId);
            IEnumerable<Int64> actual = context
                .Set<RolePermission>()
                .Where(rolePermission => rolePermission.RoleId != role.Id)
                .Select(rolePermission => rolePermission.PermissionId)
                .OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Edit_Role()
        {
            RoleView view = ObjectsFactory.CreateRoleView(role.Id);
            view.Title = role.Title += "Test";

            service.Edit(view);

            Role actual = Assert.Single(context.Set<Role>().AsNoTracking());
            Role expected = role;

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Edit_RolePermissions()
        {
            Permission permission = ObjectsFactory.CreatePermission(0);
            context.Add(permission);
            context.SaveChanges();

            RoleView view = ObjectsFactory.CreateRoleView(role.Id);
            view.Permissions = CreatePermissions();
            view.Permissions.SelectedIds.Remove(view.Permissions.SelectedIds.First());
            view.Permissions.SelectedIds.Add(permission.Id);

            service.Edit(view);

            IEnumerable<Int64> actual = context.Set<RolePermission>().AsNoTracking().Select(rolePermission => rolePermission.PermissionId).OrderBy(permissionId => permissionId);
            IEnumerable<Int64> expected = view.Permissions.SelectedIds.OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete_NullsAccountRoles()
        {
            Account account = ObjectsFactory.CreateAccount(0);
            account.RoleId = role.Id;
            account.Role = null;

            context.Add(account);
            context.SaveChanges();

            service.Delete(role.Id);

            Assert.Contains(context.Set<Account>().AsNoTracking(), model => model.Id == account.Id && model.RoleId == null);
        }

        [Fact]
        public void Delete_Role()
        {
            service.Delete(role.Id);

            Assert.Empty(context.Set<Role>());
        }

        private Role SetUpData()
        {
            Role model = ObjectsFactory.CreateRole(0);

            foreach (String controller in new[] { nameof(Accounts), nameof(Roles) })
                foreach (String action in new[] { nameof(Roles.Create), nameof(Accounts.Details) })
                    model.Permissions.Add(new RolePermission
                    {
                        Permission = new Permission
                        {
                            Area = controller == nameof(Roles) ? nameof(Area.Administration) : "",
                            Controller = controller,
                            Action = action
                        }
                    });

            context.Drop().Add(model);
            context.SaveChanges();

            return model;
        }

        private MvcTree CreatePermissions()
        {
            MvcTree expectedTree = new();
            MvcTreeNode root = new(Resource.ForString("All"));

            expectedTree.Nodes.Add(root);
            expectedTree.SelectedIds = new HashSet<Int64>(role.Permissions.Select(rolePermission => rolePermission.PermissionId));

            IEnumerable<PermissionView> permissions = role
                .Permissions
                .Select(rolePermission => rolePermission.Permission)
                .Select(permission => new PermissionView
                {
                    Id = permission.Id,
                    Area = Resource.ForArea(permission.Area),
                    Action = Resource.ForAction(permission.Action),
                    Controller = Resource.ForController($"{permission.Area}/{permission.Controller}".Trim('/'))
                });

            foreach (IGrouping<String, PermissionView> area in permissions.GroupBy(permission => permission.Area).OrderBy(permission => permission.Key.Length == 0 ? permission.First().Controller : permission.Key))
            {
                List<MvcTreeNode> nodes = new();

                foreach (IGrouping<String, PermissionView> controller in area.GroupBy(permission => permission.Controller))
                {
                    MvcTreeNode node = new(controller.Key);

                    foreach (PermissionView permission in controller)
                        node.Children.Add(new MvcTreeNode(permission.Id, permission.Action));

                    nodes.Add(node);
                }

                if (area.Key.Length == 0)
                    root.Children.AddRange(nodes);
                else
                    root.Children.Add(new MvcTreeNode(area.Key) { Children = nodes });
            }

            return expectedTree;
        }

        private IEnumerable<MvcTreeNode> GetLeafNodes(IEnumerable<MvcTreeNode> nodes)
        {
            List<MvcTreeNode> leafs = new();

            foreach (MvcTreeNode node in nodes)
                if (node.Children.Count > 0)
                    leafs.AddRange(GetLeafNodes(node.Children));
                else
                    leafs.Add(node);

            return leafs;
        }
        private IEnumerable<MvcTreeNode> GetBranchNodes(IEnumerable<MvcTreeNode> nodes)
        {
            List<MvcTreeNode> branches = nodes.Where(node => node.Children.Count > 0).ToList();

            foreach (MvcTreeNode branch in branches.ToArray())
                branches.AddRange(GetBranchNodes(branch.Children));

            return branches;
        }
    }
}
