using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Contracts.Response
{
    public readonly record struct ItemDefinitionCreationResponse
    {
        public required ItemID ItemID { get; init; }
        public required uint BaseSellPrice { get; init; }
        public required Information Information { get; init; }
    }
}