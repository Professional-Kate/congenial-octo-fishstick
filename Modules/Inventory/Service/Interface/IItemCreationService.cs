using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Contracts;

namespace IdelPog.Inventory.Service.Interface
{
    public interface IItemCreationService
    {
        public Item Create(ItemID id, uint amount);
    }
}