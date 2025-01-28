using System;
using IdelPog.Exceptions;
using IdelPog.Service;
using IdelPog.Structures.Item;

namespace IdelPog.Repository
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

        /// <summary>
        /// Creates a new <see cref="Inventory"/> class with all dependencies resolved
        /// </summary>
        /// <returns>A new <see cref="Inventory"/></returns>
        public static IInventory CreateDefault()
        {
            IRepository<InventoryID, Item> repository = new Repository<InventoryID, Item>();
            IItemFactory itemFactory = new ItemFactory();

            return new Inventory(repository, itemFactory);
        }

        public void AddAmount(InventoryID id, int amount)
        {
            AssertAmountIsPositive(amount);

            Item finalItem;

            if (_repository.Contains(id) == false)
            {
                // in cases where we don't have the item, we create it
                finalItem = _itemFactory.CreateItem(id, amount);
                _repository.Add(finalItem.ID, finalItem);
            }
            else
            {
                finalItem = RepositoryGet(id);
            }

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

        public void AddItem(InventoryID id, int startingAmount)
        {
            AssertAmountIsPositive(startingAmount);

            if (_repository.Contains(id))
            {
                throw new ArgumentException($"Error! Passed ID {id} already exists! Cannot AddItem!");
            }

            Item newItem = _itemFactory.CreateItem(id, startingAmount);
            _repository.Add(id, newItem);
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