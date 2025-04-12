using IdelPog.Engine.Repository.Inventory;
using IdelPog.Engine.Service.Information;
using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models.Item;
using IdelPog.Engine.Utilities.Builders.Item;

namespace IdelPog.Engine.Orchestration.Inventory
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    public class InventoryMediator : IInventoryMediator
    {
        private readonly IInventory _inventory;
        private readonly IMapper<InventoryID> _mapper;
        
        public InventoryMediator(IInventory inventory, IMapper<InventoryID> mapper)
        {
            _inventory = inventory;
            _mapper = mapper;
        }

        public ServiceResponse AddAmount(InventoryID inventoryID, int amount)
        {
            try
            {
                if (_inventory.Contains(inventoryID) == false)
                {
                    // if an Item doesn't exist then we create one
                    CreateItem(inventoryID, amount);
                }
                
                _inventory.AddAmount(inventoryID, amount);
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
                _inventory.RemoveAmount(inventoryID, amount);
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
            Information itemInformation = _mapper.GetInformation(inventoryID);
            
            // TODO: for now, sell price is set to 1. This is a placeholder for all items.
            Item newItem = ItemBuilder.Builder()
                .InventoryID(inventoryID)
                .Information(itemInformation)
                .SellPrice(1)
                .Amount(amount)
                .Build();
            
            _inventory.AddItem(newItem);
        }
    }
}