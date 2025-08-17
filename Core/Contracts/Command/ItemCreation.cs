using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct ItemCreation
    {
        public required ItemID ItemID { get; init; }
        public required uint Amount { get; init; }
    }
}