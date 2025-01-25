using System;
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
           if (amount <= 0)
           {
               throw new ArgumentException($"Error! Passed amount : '{amount}' is required to be a positive number.");
           }
           
           Item clonedItem = _repository.Get(item);

           clonedItem.AddAmount(amount);
           _repository.Update(item, clonedItem);
       }

        public void RemoveAmount(InventoryID item, int amount)
        {
            throw new NotImplementedException();
        }

        public void AddItem(InventoryID item, int startingAmount)
        {
            throw new NotImplementedException();
        }

        public bool Contains(InventoryID item)
        {
            throw new NotImplementedException();
        }
    }
}