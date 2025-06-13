using IdelPog.Engine.Models;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Infrastructure.Repository;
using IdelPog.Validation.Assertions;

namespace IdelPog.Engine
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory(
        IStateRepository<InventoryID, Item> stateRepository,
        IAssertFound assertFound,
        IAssertPositive assertPositive,
        IAssertNonDuplicate assertNonDuplicate)
        : IInventory
    {
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
                stateRepository.Remove(item.ID);
                return;
            }

            item.RemoveAmount(amount);
            RepositoryUpdate(item.ID, item);
        }

        public void AddItem(Item item)
        {
            AssertAmountIsPositive(item.Amount);
            assertNonDuplicate.AssertContains(item, () => Contains(item.ID));

            stateRepository.Add(item.ID, item);
        }

        public bool Contains(InventoryID item)
        {
            return stateRepository.Contains(item);
        }

        /// <summary>
        /// Asserts that the passed amount is greater than zero
        /// </summary>
        /// <param name="amount">The amount you want to verify</param>
        private void AssertAmountIsPositive(int amount)
        {
            assertPositive.AssertNumberIsPositive(amount);
        }

        /// <summary>
        /// Asserts that the passed id exists in the inventory
        /// </summary>
        /// <param name="id">The id you want to check</param>
        private void AssertItemExists(InventoryID id)
        {
            assertFound.AssertItemIsFound(id, () => Contains(id));
        } 

        private Item RepositoryGet(InventoryID id)
        {
            Item itemClone = stateRepository.Get(id);
            return itemClone;
        }

        private void RepositoryUpdate(InventoryID id, Item item)
        {
            stateRepository.Update(id, item);
        }
    }
}