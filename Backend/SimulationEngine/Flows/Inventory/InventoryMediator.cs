using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    public class InventoryMediator(IInventory inventory, IMapper<ItemID> mapper) : IInventoryMediator
    {
        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates)
        {
            
            List<InventoryUpdateDTO> updateDTOs = [];
            
            foreach (InventoryUpdate update in updates)
            {
                MutateType mutateType = MutateType.CHANGED;
                
                switch (update.Action)
                {
                    case ActionType.ADD:
                        mutateType = AddAmount(update.ItemID, update.Amount);
                        break;
                    case ActionType.REMOVE:
                        mutateType = inventory.RemoveAmount(update.ItemID, update.Amount);
                        break;
                }

                Item item = inventory.GetItem(update.ItemID);
                updateDTOs.Add(new InventoryUpdateDTO
                {
                    ItemDTO = new ItemDTO
                    {
                        Amount = item.Amount,
                        ItemID = item.ID,
                        SellPrice = item.SellPrice
                    },
                    ActionType = update.Action,
                    MutateType = mutateType
                });
                
                // TODO: Dispatch list of updateDTOs
            }
        }

        /// <summary>
        /// Adds an amount to an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="ItemID"/>
        /// </summary>
        /// <param name="itemID">The <see cref="Item"/> you want to add will have this <see cref="ItemID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <returns>A <see cref="ServiceResponse"/> object that tells you how the operation went</returns>
        /// <remarks>If the <see cref="Item"/> with the passed <see cref="ItemID"/> is not found, it will be created</remarks>
        private MutateType AddAmount(ItemID itemID, int amount)
        {
            if (inventory.Contains(itemID))
            {
                inventory.AddAmount(itemID, amount);
                return MutateType.CHANGED;
            }
            
            // if an Item doesn't exist then we create one
            CreateItem(itemID, amount);
            inventory.AddAmount(itemID, amount);
            return MutateType.CREATED;
        }

        /// <summary>
        /// creates an <see cref="Item"/> using the passed <see cref="ItemID"/> and amount
        /// </summary>
        /// <param name="itemID">The <see cref="ItemID"/> of the <see cref="Item"/> you want to create</param>
        /// <param name="amount">The amount you want the <see cref="Item"/> to have</param>
        private void CreateItem(ItemID itemID, int amount)
        {
            // TODO: should be the role of a factory
            Information itemInformation = mapper.GetInformation(itemID);

            Item newItem = ItemBuilder.Create(itemID, itemInformation)
                .Amount(amount)
                .Build();
            
            inventory.AddItem(newItem);
        }
    }
}