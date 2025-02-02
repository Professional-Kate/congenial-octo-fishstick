using System;
using IdelPog.Exceptions;
using IdelPog.Structures.Models.Item;

namespace IdelPog.Repository
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
        private readonly IRepository<InventoryID, Item> _repository;
        
        public Inventory()
        {
            _repository = Repository<InventoryID, Item>.GetInstance();
        }

        public Inventory(IRepository<InventoryID, Item> repository)
        {
            _repository = repository;
        }

        public void AddAmount(InventoryID id, int amount)
        {
            AssertAmountIsPositive(amount);

            if (_repository.Contains(id) == false)
            {
                throw new NotFoundException("Error: Item does not exist");
            }

            Item finalItem = RepositoryGet(id);

            finalItem.AddAmount(amount);
            RepositoryUpdate(id, finalItem);
        }

        public void RemoveAmount(InventoryID id, int amount)
        {
            AssertAmountIsPositive(amount);

            if (_repository.Contains(id) == false)
            {
                throw new NotFoundException("Error: Item does not exist");
            }

            Item item = RepositoryGet(id);

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
            RepositoryUpdate(item.ID, item);
        }

        public void AddItem(Item item)
        {
            AssertAmountIsPositive(item.Amount);

            if (_repository.Contains(item.ID))
            {
                throw new ArgumentException($"Error! Passed ID {item.ID} already exists! Cannot AddItem!");
            }

            _repository.Add(item.ID, item);
        }

        public bool Contains(InventoryID item)
        {
            return _repository.Contains(item);
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

        private Item RepositoryGet(InventoryID id)
        {
            Item itemClone = _repository.Get(id);
            return itemClone;
        }

        private void RepositoryUpdate(InventoryID id, Item item)
        {
            _repository.Update(id, item);
        }
    }
}