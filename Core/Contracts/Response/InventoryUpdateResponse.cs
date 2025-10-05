using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct InventoryUpdateResponse
    { 
        public required ItemInfo ItemInfo { get; init; }
        public required MutateType MutateType { get; init; }
    }
}