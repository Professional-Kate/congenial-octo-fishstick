using System;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Item;

namespace IdelPog.Orchestration
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    public class InventoryMediator : IInventoryMediator
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        private readonly IMapper<InventoryID> _mapper;
        
        public InventoryMediator()
        {
            _inventory = Inventory.CreateDefault();
            _itemFactory = new ItemFactory();
            _mapper = new Mapper<InventoryID>();
        }
        
        public InventoryMediator(IInventory inventory, IItemFactory itemFactory, IMapper<InventoryID> mapper)
        {
            _inventory = inventory;
            _itemFactory = itemFactory;
            _mapper = mapper;
        }

        public ServiceResponse AddAmount(InventoryID inventoryID, int amount)
        {
            try
            {
                if (_inventory.Contains(inventoryID) == false)
                {
                    // if an Item doesn't exist then we create one, and then add it
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
            if (_inventory.Contains(inventoryID) == false)
            {
                return ServiceResponse.Failure($"Error! Passed {inventoryID} does not exist!");
            }
            
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

        private void CreateItem(InventoryID inventoryID, int amount)
        {
            Information itemInformation = _mapper.GetInformation(inventoryID);
            
            // TODO: for now, sell price is set to 1. This is a placeholder for all items.
            Item item = _itemFactory.CreateItem(inventoryID, itemInformation, 1, amount);
            
            _inventory.AddItem(item);
        }
    }
}