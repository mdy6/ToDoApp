using ToDoApp.Services;
using System;

namespace ToDoApp.Controllers
{
    public abstract class ServicedController<TService> : AController
        where TService : IService
    {
        public TService Service { get; }

        protected ServicedController(TService service)
        {
            Service = service;
        }

        protected override void Dispose(Boolean disposing)
        {
            Service.Dispose();

            base.Dispose(disposing);
        }
    }
}
