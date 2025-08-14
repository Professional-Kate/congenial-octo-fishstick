using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;

namespace IdelPog.Inventory.Service
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
        private readonly IStateRepository<ItemID, Item> _itemRepository;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public Inventory(IStateRepository<ItemID, Item> itemRepository, IFoundAssertion foundAssertion, IUniqueAssertion uniqueAssertion)
        {
            _itemRepository = itemRepository;
            _foundAssertion = foundAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void AddAmount(ItemID id, uint amount)
        {
            AssertItemExists(id);

            Item finalItem = RepositoryGet(id);

            finalItem.Amount += amount;
            RepositoryUpdate(id, finalItem);
        }

        public MutateType RemoveAmount(ItemID id, uint amount)
        {
            AssertItemExists(id);

            Item item = RepositoryGet(id);
            uint itemAmount = item.Amount;

            if (itemAmount - amount == 0)
            {
                _itemRepository.Remove(item.ItemID);
                return MutateType.DELETED;
            }

            item.Amount -= amount;
            RepositoryUpdate(item.ItemID, item);
            return MutateType.CHANGED;
        }

        public void AddItem(Item item)
        {
            _uniqueAssertion.AssertUnique(item.ItemID, Contains(item.ItemID));

            _itemRepository.Add(item.ItemID, item);
        }

        public bool Contains(ItemID item)
        {
            return _itemRepository.Contains(item);
        }

        public Item GetItem(ItemID item)
        {
            AssertItemExists(item);
            return _itemRepository.Get(item);
        }

        /// <summary>
        /// Asserts that the passed id exists in the inventory
        /// </summary>
        /// <param name="id">The id you want to check</param>
        private void AssertItemExists(ItemID id)
        {
            _foundAssertion.AssertFound(id, Contains(id));
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