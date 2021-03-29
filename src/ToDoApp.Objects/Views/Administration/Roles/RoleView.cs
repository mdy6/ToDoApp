using AutoMapper;
using ToDoApp.Components.Tree;
using NonFactors.Mvc.Lookup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Objects
{
    public class RoleView : AView<Role>
    {
        [LookupColumn]
        [StringLength(128)]
        public String Title { get; set; }

        public MvcTree Permissions { get; set; }

        public RoleView()
        {
            Permissions = new MvcTree();
        }

        internal override void Map(Profile profile)
        {
            profile.CreateMap<Role, RoleView>().ForMember(role => role.Permissions, member => member.Ignore());
            profile.CreateMap<RoleView, Role>().ForMember(role => role.Permissions, member => member.MapFrom(_ => new List<RolePermission>()));
        }
    }
}
