using System;
using IdelPog.Structures.Models.Item;
using IdelPog.Validation;
using IdelPog.Validation.Handlers;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Repository
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
        private readonly IRepository<InventoryID, Item> _repository;
        private readonly IAssertFound _assertFound;
        private readonly IAssertPositive _assertPositive;
        
        public Inventory()
        {
            IAssertFound assertFound = new AssertFound(new ThrowHandler());
            _repository = new Repository<InventoryID, Item>(assertFound);
        }

        public Inventory(IRepository<InventoryID, Item> repository, IAssertFound assertFound, IAssertPositive assertPositive)
        {
            _repository = repository;
            _assertFound = assertFound;
            _assertPositive = assertPositive;
        }

        public void AddAmount(InventoryID id, int amount)
        {
            AssertAmountIsPositive(amount);
            AssertItemExists(id);

            Item finalItem = RepositoryGet(id);

            finalItem.AddAmount(amount);
            RepositoryUpdate(id, finalItem);
        }

        public void RemoveAmount(InventoryID id, int amount)
        {
            AssertAmountIsPositive(amount);
            AssertItemExists(id);

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
        private void AssertAmountIsPositive(int amount)
        {
            _assertPositive.AssertNumberIsPositive(amount);
        }

        /// <summary>
        /// Asserts that the passed id exists in the inventory
        /// </summary>
        /// <param name="id">The id you want to check</param>
        private void AssertItemExists(InventoryID id)
        {
            _assertFound.AssertItemIsFound(Contains(id), id);
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