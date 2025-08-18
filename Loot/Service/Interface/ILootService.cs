using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Service.Interface
{
    public interface ILootService
    { 
        public void DispatchInventoryUpdates(ItemID itemID);
    }
}