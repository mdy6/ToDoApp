using Microsoft.AspNetCore.Mvc;
using ToDoApp.Components.Lookups;
using ToDoApp.Components.Security;
using ToDoApp.Data;
using ToDoApp.Objects;
using NonFactors.Mvc.Lookup;
using System;

namespace ToDoApp.Controllers
{
    [AllowUnauthorized]
    public class Lookup : AController
    {
        private IUnitOfWork UnitOfWork { get; }

        public Lookup(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        public JsonResult Role(LookupFilter filter)
        {
            return Json(new MvcLookup<Role, RoleView>(UnitOfWork) { Filter = filter }.GetData());
        }

        protected override void Dispose(Boolean disposing)
        {
            UnitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}
