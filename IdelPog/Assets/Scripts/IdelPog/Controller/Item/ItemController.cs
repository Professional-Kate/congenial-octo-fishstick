using IdelPog.Orchestration;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Item;

namespace IdelPog.Controller
{
    /// <summary>
    /// The main control object for Item models
    /// </summary>
    /// <seealso cref="ModifyItem"/>
    public class ItemController : IItemController
    {
        private readonly IInventoryMediator _inventoryMediator;

        public ItemController(IInventoryMediator inventoryMediator)
        {
            _inventoryMediator = inventoryMediator;
        }
        
        public void ModifyItem(InventoryID id, int amount, ActionType action)
        {
            switch (action)
            {
                case ActionType.ADD:
                    _inventoryMediator.AddAmount(id, amount);
                    break;
                case ActionType.REMOVE:
                    _inventoryMediator.RemoveAmount(id, amount);
                    break;
            }
        }
    }
}