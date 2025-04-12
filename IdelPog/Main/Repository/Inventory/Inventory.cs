using IdelPog.Main.Structures.Models.Item;
using IdelPog.Main.Validation.Assertions.Interfaces;

namespace IdelPog.Main.Repository.Inventory
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
        private readonly IRepository<InventoryID, Item> _repository;
        private readonly IAssertFound _assertFound;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertNonDuplicate _assertNonDuplicate;
        
        public Inventory(IRepository<InventoryID, Item> repository, IAssertFound assertFound, IAssertPositive assertPositive, IAssertNonDuplicate assertNonDuplicate)
        {
            _repository = repository;
            _assertFound = assertFound;
            _assertPositive = assertPositive;
            _assertNonDuplicate = assertNonDuplicate;
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
            
           AssertAmountIsPositive(itemAmount - amount);
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
            _assertNonDuplicate.AssertContains(item, () => Contains(item.ID));

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
            _assertFound.AssertItemIsFound(id, () => Contains(id));
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