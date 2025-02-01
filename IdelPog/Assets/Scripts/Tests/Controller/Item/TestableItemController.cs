using IdelPog.Controller;
using IdelPog.Orchestration;

namespace Tests.Controller
{
    internal class TestableItemController : ItemController
    {
        internal TestableItemController(IInventoryMediator mediator)
        {
            InventoryMediator = mediator;
        }
    }
}