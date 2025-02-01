using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Item;

namespace IdelPog.Controller
{
    /// <see cref="ModifyItem"/>
    public interface IItemController
    {
        /// <summary>
        /// Modifies an <see cref="Item"/> with the passed internal <see cref="InventoryID"/>
        /// </summary>
        /// <param name="id">The <see cref="Item"/> you want to modify will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to modify the <see cref="Item"/> by</param>
        /// <param name="action"><see cref="ActionType"/></param>
        public void ModifyItem(InventoryID id, int amount, ActionType action);
    }
}