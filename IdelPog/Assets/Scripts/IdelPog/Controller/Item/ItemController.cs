using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Item;

namespace IdelPog.Controller
{
    /// <summary>
    /// The main control object for Item models
    /// </summary>
    /// <seealso cref="ModifyItem"/>
    public class ItemController : Singleton<ItemController>, IItemController
    {
        protected IInventoryMediator InventoryMediator = new InventoryMediator();
        
        public void ModifyItem(InventoryID id, int amount, ActionType action)
        {
            switch (action)
            {
                case ActionType.ADD:
                    InventoryMediator.AddAmount(id, amount);
                    break;
                case ActionType.REMOVE:
                    InventoryMediator.RemoveAmount(id, amount);
                    break;
            }
        }
    }
}