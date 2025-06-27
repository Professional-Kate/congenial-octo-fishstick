using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
        private readonly IStateRepository<ItemID, Item> _itemRepository;
        private readonly IAssertFound _assertFound;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertNonDuplicate _assertNonDuplicate;

        public Inventory(IStateRepository<ItemID, Item> itemRepository, IAssertFound assertFound, IAssertPositive assertPositive, IAssertNonDuplicate assertNonDuplicate)
        {
            _itemRepository = itemRepository;
            _assertFound = assertFound;
            _assertPositive = assertPositive;
            _assertNonDuplicate = assertNonDuplicate;
        }
        
        public void AddAmount(ItemID id, int amount)
        {
            AssertAmountIsPositive(amount);
            AssertItemExists(id);

            Item finalItem = RepositoryGet(id);

            finalItem.AddAmount(amount);
            RepositoryUpdate(id, finalItem);
        }

        public void RemoveAmount(ItemID id, int amount)
        {
            AssertAmountIsPositive(amount);
            AssertItemExists(id);

            Item item = RepositoryGet(id);
            int itemAmount = item.Amount;
            
           AssertAmountIsPositive(itemAmount - amount);
            if (itemAmount - amount == 0)
            {
                _itemRepository.Remove(item.ID);
                return;
            }

            item.RemoveAmount(amount);
            RepositoryUpdate(item.ID, item);
        }

        public void AddItem(Item item)
        {
            AssertAmountIsPositive(item.Amount);
            _assertNonDuplicate.AssertContains(item, () => Contains(item.ID));

            _itemRepository.Add(item.ID, item);
        }

        public bool Contains(ItemID item)
        {
            return _itemRepository.Contains(item);
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
        private void AssertItemExists(ItemID id)
        {
            _assertFound.AssertItemIsFound(id, () => Contains(id));
        } 

        private Item RepositoryGet(ItemID id)
        {
            Item itemClone = _itemRepository.Get(id);
            return itemClone;
        }

        private void RepositoryUpdate(ItemID id, Item item)
        {
            _itemRepository.Update(id, item);
        }
    }
}