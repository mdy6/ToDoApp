using ToDoApp.Data;

namespace ToDoApp.Services
{
    public abstract class AService : IService
    {
        protected IUnitOfWork UnitOfWork { get; }

        protected AService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}
