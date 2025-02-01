using System;
using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;

namespace IdelPog.Controller
{
    /// <summary>
    /// The main control object for Item models
    /// </summary>
    /// <seealso cref="ModifyItem"/>
    public class ItemController : Singleton<ItemController>, IItemController
    {
        protected IInventoryMediator InventoryMediator = Orchestration.InventoryMediator.CreateDefault();
        
        public void ModifyItem(InventoryID id, ActionType action)
        {
            throw new NotImplementedException();
        }
    }
}