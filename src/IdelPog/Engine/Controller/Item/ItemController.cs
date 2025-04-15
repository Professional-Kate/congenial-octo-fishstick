using IdelPog.Engine.Orchestration;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Controller
{
    /// <summary>
    /// The main control object for Item models
    /// </summary>
    /// <seealso cref="ModifyItem"/>
    public class ItemController(IInventoryMediator inventoryMediator) : IItemController
    {
        public ServiceResponse ModifyItem(InventoryID id, int amount, ActionType action)
        {
            ServiceResponse serviceResponse = ServiceResponse.Success();
            
            switch (action)
            {
                case ActionType.ADD:
                    serviceResponse =inventoryMediator.AddAmount(id, amount);
                    break;
                case ActionType.REMOVE:
                    serviceResponse =inventoryMediator.RemoveAmount(id, amount);
                    break;
            }

            return serviceResponse;
        }
    }
}