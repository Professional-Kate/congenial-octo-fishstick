using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Service.Interface;

namespace IdelPog.HarvestNode.Runtime.System
{
    public sealed class HarvestNodeLootService : IHarvestNodeLootService
    {
        private readonly ILootService<ItemID> _itemLootService;
        private readonly ILootService<LocationID> _locationLootService;

        public HarvestNodeLootService(ILootService<ItemID> itemLootService, ILootService<LocationID> locationLootService)
        {
            _itemLootService = itemLootService;
            _locationLootService = locationLootService;
        }

        public IReadOnlyList<InventoryUpdate> GenerateInventoryUpdates(Contracts.HarvestNode harvestNode)
        {
            List<InventoryUpdate> inventoryUpdates = [];

            TryAddLoot(_itemLootService, harvestNode.ItemID, inventoryUpdates);
            TryAddLoot(_locationLootService, harvestNode.LocationID, inventoryUpdates);
            
            return inventoryUpdates;
        }
        
        private static void TryAddLoot<TID>(ILootService<TID> lootService, TID id, List<InventoryUpdate> inventoryUpdates) where TID : Enum
        {
            try
            {
                if (lootService.ShouldGrant(id) == false)
                {
                    return;
                }
                
                ItemID itemID = lootService.GenerateItemID(id);
                inventoryUpdates.Add(GenerateUpdate(itemID));
            }
            catch (NotFoundException<TID>)
            {
                // suppressed heehee haha
            }
        }
        
        private static InventoryUpdate GenerateUpdate(ItemID itemID)
        {
            return new InventoryUpdate
            {
                ItemID = itemID,
                ActionType = ActionType.ADD,
                Amount = 1
            };
        }

    }
}