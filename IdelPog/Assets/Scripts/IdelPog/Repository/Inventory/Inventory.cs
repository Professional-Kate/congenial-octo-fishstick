using System;
using IdelPog.Service;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;

namespace IdelPog.Repository.Inventory
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
       private readonly IRepository<InventoryID, Item> _repository;
       private readonly IItemFactory _itemFactory; 

       public Inventory(IRepository<InventoryID, Item> repository, IItemFactory itemFactory)
       {
           _repository = repository;
           _itemFactory = itemFactory;
       }

       public IInventory CreateDefault()
       {
           IRepository<InventoryID, Item> repository = new Repository<InventoryID, Item>();
           IItemFactory itemFactory = new ItemFactory();
           
           return new Inventory(repository, itemFactory);
       }

       public void AddAmount(InventoryID item, int amount)
       {
           ModifyAmount(item, amount, ActionType.ADD);
       }

        public void RemoveAmount(InventoryID item, int amount)
        {
            ModifyAmount(item, amount, ActionType.REMOVE);
        }

        public void AddItem(InventoryID item, int startingAmount)
        {
            AssertAmountIsPositive(startingAmount);
            
            if (_repository.Contains(item))
            {
                throw new ArgumentException($"Error! Passed ID {item} already exists! Cannot AddItem!");
            }
            
            Item newItem = _itemFactory.CreateItem(item, startingAmount);
            _repository.Add(item, newItem);
        }

        /// <summary>
        /// Asserts that the passed amount is greater than zero
        /// </summary>
        /// <param name="amount">The amount you want to verify</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed amount is 0 or less</exception>
        private static void AssertAmountIsPositive(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException($"Error! Passed amount : '{amount}' is required to be a positive number.");
            }
        }
        
        /// <summary>
        /// Modifies the amount of the passed <see cref="InventoryID"/>
        /// </summary>
        /// <param name="item">This <see cref="InventoryID"/> will match the <see cref="InventoryID"/> of an <see cref="Item"/> in the repository</param>
        /// <param name="amount">The amount to modify the <see cref="Item"/>s amount by</param>
        /// <param name="actionType"><see cref="ActionType"/></param>
        private void ModifyAmount(InventoryID item, int amount, ActionType actionType)
        {
            AssertAmountIsPositive(amount);
            
            Item clonedItem = _repository.Get(item);

            switch (actionType)
            {
                case ActionType.ADD:
                    clonedItem.AddAmount(amount);
                    _repository.Update(item, clonedItem);
                    break;
                case ActionType.REMOVE:
                    RemoveAmountHandler(clonedItem, amount);
                    break;
            }
        }

        /// <summary>
        /// Handles removing an amount from an <see cref="Item"/>
        /// </summary>
        /// <param name="item">The <see cref="Item"/> you want to remove an amount from</param>
        /// <param name="amount">The amount you want to remove</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed amount would cause the <see cref="Item"/>s amount to be less than zero</exception>
        /// <remarks>
        /// This method will handle Updating the Repository with the new state of the <see cref="Item"/>. In cases where the <see cref="Item"/> is removed, because the <see cref="Item"/>'s amount is zero, an Update will not be called
        /// </remarks>
        private void RemoveAmountHandler(Item item, int amount)
        {
            int itemAmount = item.Amount;

            if (itemAmount < amount)
            {
                throw new ArgumentException($"Error! Cannot remove amount : '{amount}', item's amount is too low: {item.Amount}.");
            }

            if (itemAmount - amount == 0)
            {
                _repository.Remove(item.ID);
                return;
            }
            
            item.RemoveAmount(amount);
            _repository.Update(item.ID, item);
        }
    }
}