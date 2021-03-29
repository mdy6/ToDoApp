using ToDoApp.Data;
using NonFactors.Mvc.Lookup;
using NSubstitute;
using Xunit;

namespace ToDoApp.Controllers
{
    public class LookupTests : ControllerTests
    {
        private Lookup controller;
        private IUnitOfWork unitOfWork;

        public LookupTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            controller = Substitute.ForPartsOf<Lookup>(unitOfWork);
        }
        public override void Dispose()
        {
            controller.Dispose();
            unitOfWork.Dispose();
        }

        [Fact]
        public void Role_Lookup()
        {
            LookupData actual = Assert.IsType<LookupData>(controller.Role(new LookupFilter()).Value);

            Assert.NotEmpty(actual.Columns);
        }

        [Fact]
        public void Dispose_UnitOfWork()
        {
            controller.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }
    }
}
