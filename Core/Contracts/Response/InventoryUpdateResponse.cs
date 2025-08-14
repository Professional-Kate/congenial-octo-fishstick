using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct InventoryUpdateResponse
    {
        public required ItemInfo ItemInfo { get; init; }
        public required ActionType ActionType { get; init; }
        public required MutateType MutateType { get; init; }
    }
}