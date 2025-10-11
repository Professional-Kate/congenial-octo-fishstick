using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Contracts.Response
{
    public readonly record struct InventoryUpdateResponse
    { 
        public required ItemInfo ItemInfo { get; init; }
        public required MutateType MutateType { get; init; }
    }
}