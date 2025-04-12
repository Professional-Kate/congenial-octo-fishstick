using IdelPog.Main.Orchestration.Inventory;
using IdelPog.Main.Structures;
using IdelPog.Main.Structures.Enums;
using IdelPog.Main.Structures.Models.Item;

namespace IdelPog.Main.Controller.Item
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
        
        public ServiceResponse ModifyItem(InventoryID id, int amount, ActionType action)
        {
            ServiceResponse serviceResponse = ServiceResponse.Success();
            
            switch (action)
            {
                case ActionType.ADD:
                    serviceResponse =_inventoryMediator.AddAmount(id, amount);
                    break;
                case ActionType.REMOVE:
                    serviceResponse =_inventoryMediator.RemoveAmount(id, amount);
                    break;
            }

            return serviceResponse;
        }
    }
}