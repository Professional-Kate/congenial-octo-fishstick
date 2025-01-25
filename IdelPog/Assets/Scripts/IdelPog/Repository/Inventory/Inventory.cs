using System;
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

       public Inventory(IRepository<InventoryID, Item> repository)
       {
           _repository = repository;
       }

       public IInventory CreateDefault()
       {
           IRepository<InventoryID, Item> repository = new Repository<InventoryID, Item>();
           return new Inventory(repository);
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
            throw new NotImplementedException();
        }

        public bool Contains(InventoryID item)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Modifies the amount of the passed <see cref="InventoryID"/>
        /// </summary>
        /// <param name="item">This <see cref="InventoryID"/> will match the <see cref="InventoryID"/> of an <see cref="Item"/> in the repository</param>
        /// <param name="amount">The amount to modify the <see cref="Item"/>s amount by</param>
        /// <param name="actionType"><see cref="ActionType"/></param>
        private void ModifyAmount(InventoryID item, int amount, ActionType actionType)
        {
            if (amount <= 0)
            {
                throw new ArgumentException($"Error! Passed amount : '{amount}' is required to be a positive number.");
            }
           
            Item clonedItem = _repository.Get(item);

            switch (actionType)
            {
                case ActionType.ADD:
                    clonedItem.AddAmount(amount);
                    break;
                case ActionType.REMOVE:
                    clonedItem.RemoveAmount(amount);
                    break;
            }
            
            _repository.Update(item, clonedItem);
        }
    }
}