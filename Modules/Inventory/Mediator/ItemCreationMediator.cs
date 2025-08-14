using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public class ItemCreationMediator<TItem> : IBatchMediator<TItem>
    {
        private readonly IInventory _inventory;

        public ItemCreationMediator(IInventory inventory)
        {
            _inventory = inventory;
        }

        public void HandleMessages(IReadOnlyList<TItem> messages)
        {
            throw new NotImplementedException();
        }
    }
}