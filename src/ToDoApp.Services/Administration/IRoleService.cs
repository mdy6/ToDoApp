using ToDoApp.Components.Tree;
using ToDoApp.Objects;
using System;
using System.Linq;

namespace ToDoApp.Services
{
    public interface IRoleService : IService
    {
        IQueryable<RoleView> GetViews();
        RoleView? GetView(Int64 id);

        void Seed(MvcTree permissions);
        void Create(RoleView view);
        void Edit(RoleView view);
        void Delete(Int64 id);
    }
}
