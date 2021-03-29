using ToDoApp.Objects;
using System;
using System.Collections.Generic;

namespace ToDoApp
{
    public static class ObjectsFactory
    {
        public static Account CreateAccount(Int64 id)
        {
            return new()
            {
                Username = $"Username{id}",
                Passhash = $"Passhash{id}",
                Email = $"{id}@tests.com",
                IsLocked = false,
                RecoveryToken = $"Token{id}",
                RecoveryTokenExpiration = DateTime.Now.AddMinutes(5),
                Role = CreateRole(id)
            };
        }

        public static AccountCreateView CreateAccountCreateView(Int64 id)
        {
            return new()
            {
                Id = id,
                Username = $"Username{id}",
                Password = $"Password{id}",
                Email = $"{id}@tests.com",
                RoleId = id
            };
        }

        public static AccountEditView CreateAccountEditView(Int64 id)
        {
            return new()
            {
                Id = id,
                Username = $"Username{id}",
                Email = $"{id}@tests.com",
                IsLocked = true,
                RoleId = id
            };
        }

        public static AccountLoginView CreateAccountLoginView(Int64 id)
        {
            return new()
            {
                Id = id,
                Username = $"Username{id}",
                Password = $"Password{id}"
            };
        }

        public static AccountRecoveryView CreateAccountRecoveryView(Int64 id)
        {
            return new()
            {
                Id = id,
                Email = $"{id}@tests.com"
            };
        }

        public static AccountResetView CreateAccountResetView(Int64 id)
        {
            return new()
            {
                Id = id,
                Token = $"Token{id}",
                NewPassword = $"NewPassword{id}"
            };
        }

        public static AccountView CreateAccountView(Int64 id)
        {
            return new()
            {
                Id = id,
                Username = $"Username{id}",
                Email = $"{id}@tests.com",
                IsLocked = true,
                RoleTitle = $"Title{id}"
            };
        }

        public static Permission CreatePermission(Int64 id)
        {
            return new()
            {
                Area = $"Area{id}",
                Action = $"Action{id}",
                Controller = $"Controller{id}"
            };
        }

        public static ProfileDeleteView CreateProfileDeleteView(Int64 id)
        {
            return new()
            {
                Id = id,
                Password = $"Password{id}"
            };
        }

        public static ProfileEditView CreateProfileEditView(Int64 id)
        {
            return new()
            {
                Id = id,
                Email = $"{id}@tests.com",
                Username = $"Username{id}",
                Password = $"Password{id}",
                NewPassword = $"NewPassword{id}"
            };
        }

        public static Role CreateRole(Int64 id)
        {
            return new()
            {
                Title = $"Title{id}",
                Accounts = new List<Account>(),
                Permissions = new List<RolePermission>()
            };
        }

        public static RolePermission CreateRolePermission(Int64 id)
        {
            return new()
            {
                RoleId = id,
                Role = CreateRole(id),
                PermissionId = id,
                Permission = CreatePermission(id)
            };
        }

        public static RoleView CreateRoleView(Int64 id)
        {
            return new()
            {
                Id = id,
                Title = $"Title{id}"
            };
        }
    }
}
