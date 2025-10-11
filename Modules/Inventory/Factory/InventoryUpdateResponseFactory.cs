using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public sealed class InventoryUpdateResponseFactory : IInventoryUpdateResponseFactory
    {
        public InventoryUpdateResponse Create(ItemInfo itemInfo, MutateType mutateType)
        {
            return new InventoryUpdateResponse
            {
                ItemInfo = itemInfo,
                MutateType = mutateType
            };
        }
    }
}