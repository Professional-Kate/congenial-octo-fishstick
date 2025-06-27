using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Enums;
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
            foreach (InventoryUpdate update in updates)
            {
                switch (update.Action)
                {
                    case ActionType.ADD:
                        AddAmount(update.ItemID, update.Amount);
                        break;
                    case ActionType.REMOVE:
                        RemoveAmount(update.ItemID, update.Amount);
                        break;
                }
            }
        }

        /// <summary>
        /// Adds an amount to an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="ItemID"/>
        /// </summary>
        /// <param name="itemID">The <see cref="Item"/> you want to add will have this <see cref="ItemID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <returns>A <see cref="ServiceResponse"/> object that tells you how the operation went</returns>
        /// <remarks>If the <see cref="Item"/> with the passed <see cref="ItemID"/> is not found, it will be created</remarks>
        private ServiceResponse AddAmount(ItemID itemID, int amount)
        {
            try
            {
                if (inventory.Contains(itemID) == false)
                {
                    // if an Item doesn't exist then we create one
                    CreateItem(itemID, amount);
                }
                
                inventory.AddAmount(itemID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        /// <summary>
        /// Removes an amount from an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="ItemID"/>
        /// </summary>
        /// <param name="itemID">The <see cref="Item"/> you want to remove amount from will have this <see cref="ItemID"/></param>
        /// <param name="amount">The amount you want to remove</param>
        /// <returns>A <see cref="ServiceResponse"/> object that tells you how the operation went</returns>
        /// <remarks>If the amount on the <see cref="Item"/> is exactly 0, after removing the passed amount, it will be removed from the Inventory</remarks>
        private ServiceResponse RemoveAmount(ItemID itemID, int amount)
        {
            try
            {
                inventory.RemoveAmount(itemID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }

        /// <summary>
        /// creates an <see cref="Item"/> using the passed <see cref="ItemID"/> and amount
        /// </summary>
        /// <param name="itemID">The <see cref="ItemID"/> of the <see cref="Item"/> you want to create</param>
        /// <param name="amount">The amount you want the <see cref="Item"/> to have</param>
        private void CreateItem(ItemID itemID, int amount)
        {
            Information itemInformation = mapper.GetInformation(itemID);

            Item newItem = ItemBuilder.Create(itemID, itemInformation)
                .Amount(amount)
                .Build();
            
            inventory.AddItem(newItem);
        }
    }
}