using IdelPog.Structures;
using IdelPog.Structures.Item;

namespace IdelPog.Orchestration.Inventory
{
    public interface IInventoryMediator
    {
        public ServiceResponse AddItem(InventoryID inventoryID, int amount);
        
        public ServiceResponse RemoveItem(InventoryID inventoryID, int amount);
    }
}