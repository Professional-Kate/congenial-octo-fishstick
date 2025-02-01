using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;

namespace IdelPog.Controller
{
    /// <see cref="ModifyItem"/>
    public interface IItemController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="action"></param>
        public void ModifyItem(InventoryID id, ActionType action);
    }
}