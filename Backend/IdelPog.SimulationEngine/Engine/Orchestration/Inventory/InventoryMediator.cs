using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Orchestration
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    public class InventoryMediator(IInventory inventory, IMapper<InventoryID> mapper) : IInventoryMediator
    {
        public ServiceResponse AddAmount(InventoryID inventoryID, int amount)
        {
            try
            {
                if (inventory.Contains(inventoryID) == false)
                {
                    // if an Item doesn't exist then we create one
                    CreateItem(inventoryID, amount);
                }
                
                inventory.AddAmount(inventoryID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        public ServiceResponse RemoveAmount(InventoryID inventoryID, int amount)
        {
            try
            {
                inventory.RemoveAmount(inventoryID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }

        /// <summary>
        /// creates an <see cref="Item"/> using the passed <see cref="InventoryID"/> and amount
        /// </summary>
        /// <param name="inventoryID">The <see cref="InventoryID"/> of the <see cref="Item"/> you want to create</param>
        /// <param name="amount">The amount you want the <see cref="Item"/> to have</param>
        private void CreateItem(InventoryID inventoryID, int amount)
        {
            Information itemInformation = mapper.GetInformation(inventoryID);

            Item newItem = ItemBuilder.Create(inventoryID, itemInformation)
                .Amount(amount)
                .Build();
            
            inventory.AddItem(newItem);
        }
    }
}