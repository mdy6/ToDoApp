using System;

namespace ToDoApp.Components.Security
{
    public interface IAuthorization
    {
        Boolean IsGrantedFor(Int64 accountId, String permission);

        void Refresh(IServiceProvider services);
    }
}
